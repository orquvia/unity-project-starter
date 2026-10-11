using System;
using System.Reflection;
using NUnit.Framework;
using Orquvia.Starter.Bootstrap;
using Orquvia.Starter.Domain;
using Orquvia.Starter.Infrastructure;
using Orquvia.Starter.Presentation;
using Orquvia.Starter.UseCase;
using UnityEditor;
using VContainer;
using VContainer.Unity;

namespace Orquvia.Starter.Tests.Bootstrap
{
    public sealed class AppLifetimeScopeTests
    {
        private const string PrefabPath = "Assets/Game/Settings/AppLifetimeScope.prefab";
        private const string SettingsPath = "Assets/Game/Settings/VContainerSettings.asset";

        private AppLifetimeScope _prefab;
        private IObjectResolver _container;

        private IObjectResolver Container => _container ??= BuildContainer(_prefab);

        [SetUp]
        public void SetUp()
        {
            _prefab = AssetDatabase.LoadAssetAtPath<AppLifetimeScope>(PrefabPath);
            Assert.That(_prefab, Is.Not.Null, $"{PrefabPath} が見つかりません");
        }

        [TearDown]
        public void TearDown()
        {
            _container?.Dispose();
            _container = null;
        }

        // --- 設定アセットの結線 ---

        [Test]
        public void VContainerSettings_UsesAppLifetimeScopeAsRoot()
        {
            var settings = AssetDatabase.LoadAssetAtPath<VContainerSettings>(SettingsPath);

            Assert.That(settings, Is.Not.Null, $"{SettingsPath} が見つかりません");
            Assert.That(settings.RootLifetimeScope, Is.EqualTo(_prefab));
        }

        [Test]
        public void Prefab_HasConfigAssigned()
        {
            Assert.That(GetConfig(_prefab), Is.Not.Null);
        }

        // --- コンテナの構成 ---

        [TestCase(typeof(GameRules))]
        [TestCase(typeof(GameSession))]
        [TestCase(typeof(IReadOnlyGameSession))]
        [TestCase(typeof(StartGameUseCase))]
        [TestCase(typeof(HitTargetUseCase))]
        [TestCase(typeof(AdvanceGameUseCase))]
        [TestCase(typeof(IGameLogger))]
        [TestCase(typeof(ISceneNavigator))]
        [TestCase(typeof(GameFlow))]
        [TestCase(typeof(ITimeProvider))]
        public void Registration_ResolvesAsSingleton(Type type)
        {
            var first = Container.Resolve(type);
            var second = Container.Resolve(type);

            Assert.That(first, Is.InstanceOf(type));
            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void Ports_AreBoundToUnityImplementations()
        {
            Assert.That(Container.Resolve<IGameLogger>(), Is.InstanceOf<UnityGameLogger>());
            Assert.That(Container.Resolve<ISceneNavigator>(), Is.InstanceOf<UnitySceneNavigator>());
            Assert.That(Container.Resolve<ITimeProvider>(), Is.InstanceOf<UnityTimeProvider>());
        }

        [Test]
        public void GameSession_IsSharedWithReadOnlyView()
        {
            // ずれると Result 画面が別のセッション（スコア 0）を読んでしまう
            Assert.That(
                Container.Resolve<IReadOnlyGameSession>(),
                Is.SameAs(Container.Resolve<GameSession>()));
        }

        [Test]
        public void UseCases_OperateOnSharedSession()
        {
            var session = Container.Resolve<IReadOnlyGameSession>();
            var rules = Container.Resolve<GameRules>();

            Container.Resolve<StartGameUseCase>().Execute();
            Container.Resolve<HitTargetUseCase>().Execute();

            Assert.That(session.Status, Is.EqualTo(GameStatus.Running));
            Assert.That(session.Score, Is.EqualTo(rules.PointsPerHit));
        }

        [Test]
        public void GameRules_ComeFromAssignedConfig()
        {
            var expected = GetConfig(_prefab).ToRules();
            var actual = Container.Resolve<GameRules>();

            Assert.That(actual.DurationSeconds, Is.EqualTo(expected.DurationSeconds));
            Assert.That(actual.PointsPerHit, Is.EqualTo(expected.PointsPerHit));
        }

        // LifetimeScope.Build() はルートスコープの生成など Play モードを前提にした処理を含むため、
        // 実際の prefab の Configure だけを呼び、素の ContainerBuilder で組み立てる。
        private static IObjectResolver BuildContainer(AppLifetimeScope scope)
        {
            var configure = typeof(AppLifetimeScope).GetMethod(
                "Configure",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var builder = new ContainerBuilder();

            configure.Invoke(scope, new object[] { builder });

            return builder.Build();
        }

        private static GameConfigAsset GetConfig(AppLifetimeScope scope)
        {
            return new SerializedObject(scope)
                .FindProperty("config")
                .objectReferenceValue as GameConfigAsset;
        }
    }
}

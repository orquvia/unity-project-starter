using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Orquvia.Starter.Domain;
using Orquvia.Starter.Infrastructure;
using Orquvia.Starter.Presentation;
using Orquvia.Starter.UseCase;

namespace Orquvia.Starter.Tests.Architecture
{
    // asmdef の参照設定が緩められて層の依存ルールが崩れていないかを、
    // コンパイル済みアセンブリが実際に参照しているアセンブリから検証する。
    // Bootstrap はコンポジションルートなので対象外。
    public sealed class LayerDependencyTests
    {
        // 検証の対象にする参照。.NET の標準ライブラリなどは含めない。
        private static readonly string[] TrackedPrefixes =
        {
            "Orquvia.",
            "UnityEngine",
            "UnityEditor",
            "VContainer",
            "UniTask"
        };

        [Test]
        public void Domain_DependsOnNothing()
        {
            AssertDependencies(typeof(GameSession).Assembly);
        }

        [Test]
        public void UseCase_DependsOnlyOnDomain()
        {
            AssertDependencies(
                typeof(StartGameUseCase).Assembly,
                "Orquvia.Starter.Domain");
        }

        [Test]
        public void Presentation_DoesNotDependOnInfrastructureOrBootstrap()
        {
            AssertDependencies(
                typeof(GameFlow).Assembly,
                "Orquvia.Starter.Domain",
                "Orquvia.Starter.UseCase",
                "UnityEngine*",
                "VContainer",
                "UniTask");
        }

        [Test]
        public void Infrastructure_DoesNotDependOnPresentationOrBootstrap()
        {
            AssertDependencies(
                typeof(UnitySceneNavigator).Assembly,
                "Orquvia.Starter.Domain",
                "Orquvia.Starter.UseCase",
                "UnityEngine*",
                "UniTask");
        }

        // allowed のうち末尾が "*" のものは前方一致として扱う
        private static void AssertDependencies(Assembly assembly, params string[] allowed)
        {
            var violations = assembly.GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .Where(name => TrackedPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal)))
                .Where(name => !allowed.Any(pattern => Matches(name, pattern)))
                .ToArray();

            Assert.That(
                violations,
                Is.Empty,
                $"{assembly.GetName().Name} に許可されていない依存があります");
        }

        private static bool Matches(string name, string pattern)
        {
            return pattern.EndsWith("*", StringComparison.Ordinal)
                ? name.StartsWith(pattern.Substring(0, pattern.Length - 1), StringComparison.Ordinal)
                : name == pattern;
        }
    }
}

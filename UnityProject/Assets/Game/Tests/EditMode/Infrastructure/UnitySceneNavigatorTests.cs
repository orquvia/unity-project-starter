using System;
using System.Linq;
using NUnit.Framework;
using Orquvia.Starter.Infrastructure;
using Orquvia.Starter.UseCase;
using UnityEditor;

namespace Orquvia.Starter.Tests.Infrastructure
{
    public sealed class UnitySceneNavigatorTests
    {
        private static readonly SceneId[] AllSceneIds = (SceneId[])Enum.GetValues(typeof(SceneId));

        [TestCaseSource(nameof(AllSceneIds))]
        public void ScenePath_IsEnabledInBuildSettings(SceneId sceneId)
        {
            var assetPath = $"Assets/{UnitySceneNavigator.ToScenePath(sceneId)}.unity";

            Assert.That(
                AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath),
                Is.Not.Null,
                $"{assetPath} が存在しません");
            Assert.That(
                EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path == assetPath),
                Is.True,
                $"{assetPath} が Build Settings で有効になっていません");
        }

        [Test]
        public void ToScenePath_UndefinedId_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => UnitySceneNavigator.ToScenePath((SceneId)(-1)));
        }
    }
}

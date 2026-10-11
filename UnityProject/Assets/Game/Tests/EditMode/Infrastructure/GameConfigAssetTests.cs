using NUnit.Framework;
using Orquvia.Starter.Infrastructure;
using UnityEditor;
using UnityEngine;

namespace Orquvia.Starter.Tests.Infrastructure
{
    public sealed class GameConfigAssetTests
    {
        [Test]
        public void ToRules_DefaultValues()
        {
            var asset = ScriptableObject.CreateInstance<GameConfigAsset>();

            try
            {
                var rules = asset.ToRules();

                Assert.That(rules.DurationSeconds, Is.EqualTo(20f));
                Assert.That(rules.PointsPerHit, Is.EqualTo(10));
            }
            finally
            {
                Object.DestroyImmediate(asset);
            }
        }

        [Test]
        public void ProjectAssets_ProduceValidRules()
        {
            // [Min] はインスペクター上の入力しか制限しないので、実際のアセットの値を検証する
            var guids = AssetDatabase.FindAssets($"t:{nameof(GameConfigAsset)}", new[] { "Assets" });

            Assert.That(guids, Is.Not.Empty);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<GameConfigAsset>(path);

                Assert.DoesNotThrow(() => asset.ToRules(), path);
            }
        }
    }
}

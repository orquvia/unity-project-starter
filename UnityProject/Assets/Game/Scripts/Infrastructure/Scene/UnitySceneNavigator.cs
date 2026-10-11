using System;
using System.Threading;
using Orquvia.Starter.UseCase;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Orquvia.Starter.Infrastructure
{
    public sealed class UnitySceneNavigator : ISceneNavigator
    {
        public async Task LoadAsync(
            SceneId sceneId,
            CancellationToken ct)
        {
            string path = ToScenePath(sceneId);

            var operation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Single);

            if (operation == null)
            {
                throw new InvalidOperationException($"Failed to load scene: {path}");
            }

            await operation.WithCancellation(ct);
        }

        // Build Settings に表示されるパス（"Assets/" と拡張子を除いたもの）
        internal static string ToScenePath(SceneId sceneId)
        {
            return sceneId switch
            {
                SceneId.Title => "Game/Scene/Title",
                SceneId.Game => "Game/Scene/Game",
                SceneId.Result => "Game/Scene/Result",
                _ => throw new ArgumentOutOfRangeException(nameof(sceneId))
            };
        }
    }
}

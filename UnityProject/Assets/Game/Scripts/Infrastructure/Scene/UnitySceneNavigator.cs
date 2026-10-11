using System;
using System.Threading;
using UnityEngine;
using Orquvia.Starter.UseCase;
using Orquvia.Starter.Bootstrap;
using System.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Qrquvia.Starter.Infrastructure
{
    public sealed class UnitySceneNavigator : ISceneNavigator
    {
        private readonly AppLifetimeScope _rootScope;

        public UnitySceneNavigator(AppLifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }

        public async Task LoadAsync(
            SceneId sceneId,
            CancellationToken ct)
        {
            string path = sceneId switch
            {
                SceneId.Title => "Game/Scene/Title",
                SceneId.Game => "Game/Scene/Game",
                SceneId.Result => "Game/Scene/Result",
                _ => throw new ArgumentOutOfRangeException(nameof(sceneId))
            };

            using (LifetimeScope.EnqueueParent(_rootScope))
            {
                var operation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Single);

                if (operation == null)
                {
                    throw new InvalidOperationException($"Failed to load scene: {path}");
                }

                await operation.WithCancellation(ct);
            }
        }
    }
}

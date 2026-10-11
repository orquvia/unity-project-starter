using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orquvia.Starter.UseCase;

namespace Orquvia.Starter.Tests.Fakes
{
    public sealed class FakeSceneNavigator : ISceneNavigator
    {
        public List<SceneId> Requests { get; } = new();

        // 既定では即座に完了する。完了タイミングや失敗を制御したいテストで差し替える。
        public Func<SceneId, CancellationToken, Task> Handler { get; set; }
            = (_, _) => Task.CompletedTask;

        public Task LoadAsync(SceneId sceneId, CancellationToken ct)
        {
            Requests.Add(sceneId);
            return Handler(sceneId, ct);
        }
    }
}

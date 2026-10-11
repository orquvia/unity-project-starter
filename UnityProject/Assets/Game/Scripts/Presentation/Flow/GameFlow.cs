using System;
using System.Threading;
using Orquvia.Starter.UseCase;
using Cysharp.Threading.Tasks;

namespace Orquvia.Starter.Presentation
{
    public sealed class GameFlow
        : IDisposable
    {
        private readonly ISceneNavigator _scenes;
        private readonly IGameLogger _logger;
        private readonly CancellationTokenSource _cts = new();

        private bool transitioning;

        public GameFlow(
            ISceneNavigator scenes,
            IGameLogger logger)
        {
            _scenes = scenes;
            _logger = logger;
        }

        public void RequestStartGame() => NavigateAsync(SceneId.Game).Forget();
        public void RequestResult() => NavigateAsync(SceneId.Result).Forget();
        public void RequestTitle() => NavigateAsync(SceneId.Title).Forget();

        private async UniTask NavigateAsync(SceneId destination)
        {
            if (transitioning)
            {
                return;
            }

            transitioning = true;

            try
            {
                await _scenes.LoadAsync(destination, _cts.Token);
                _logger.Info($"Loaded scene: {destination}");
            }
            catch (OperationCanceledException)
                when (_cts.IsCancellationRequested)
            {
                // GameFlow の破棄（アプリ終了）に伴うキャンセル。
                // シーンロード自体は中断できないため、完了待ちを打ち切るだけで何もしない。
            }
            catch (Exception ex)
            {
                _logger.Error("Scene transition failed", ex);
            }
            finally
            {
                transitioning = false;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}

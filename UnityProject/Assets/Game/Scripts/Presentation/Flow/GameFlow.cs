using System.Threading;
using Orquvia.Starter.UseCase;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace Orquvia.Starter.Presentation
{
    public sealed class GameFlow
    {
        private readonly ISceneNavigator _scenes;
        private readonly StartGameUseCase _startGame;
        private readonly IGameLogger _logger;
        private readonly CancellationTokenSource _cts = new();

        private bool transitioning;

        public GameFlow(
            ISceneNavigator scenes,
            StartGameUseCase startGame,
            IGameLogger logger)
        {
            _scenes = scenes;
            _startGame = startGame;
            _logger = logger;
        }

        public void RequestStartGame() => NavigateAsync(SceneId.Game, true).Forget();
        public void RequestResult() => NavigateAsync(SceneId.Result, false).Forget();
        public void RequestTitle() => NavigateAsync(SceneId.Title, false).Forget();

        private async UniTask NavigateAsync(
            SceneId destination,
            bool startGame)
        {
            if (transitioning)
            {
                return;
            }

            transitioning = true;

            try
            {
                if (startGame)
                {
                    _startGame.Execute();
                }

                await _scenes.LoadAsync(destination, _cts.Token);
                _logger.Info($"Loaded scene: {destination}");
            }
            catch (OperationCanceledException)
                when (_cts.IsCancellationRequested)
            {
                // TODO: 正常なキャンセル
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

using System;
using UnityEngine;
using VContainer.Unity;
using Orquvia.Starter.Domain;
using Orquvia.Starter.UseCase;

namespace Orquvia.Starter.Presentation
{
    public sealed class GamePresenter
        : IStartable, ITickable, IDisposable
    {
        private readonly GameView _view;
        private readonly IReadOnlyGameSession _session;
        private readonly HitTargetUseCase _hitTarget;
        private readonly AdvanceGameUseCase _advanceGame;
        private readonly StartGameUseCase _startGame;
        private readonly GameFlow _flow;

        private bool resultRequested;

        public GamePresenter(
            GameView view,
            IReadOnlyGameSession session,
            HitTargetUseCase hitTarget,
            AdvanceGameUseCase advanceGame,
            StartGameUseCase startGame,
            GameFlow flow)
        {
            _view = view;
            _session = session;
            _hitTarget = hitTarget;
            _advanceGame = advanceGame;
            _startGame = startGame;
            _flow = flow;
        }

        public void Start()
        {
            _startGame.Execute();
            _view.TargetButton.onClick.AddListener(OnHit);
            Refresh();
        }

        public void Tick()
        {
            if (resultRequested)
            {
                return;
            }

            var finished = _advanceGame.Execute(Time.deltaTime);
            Refresh();

            if (finished)
            {
                resultRequested = true;
                _view.SetInteractable(false);
                _flow.RequestResult();
            }
        }

        public void Dispose()
        {
            _view.TargetButton.onClick.RemoveListener(OnHit);
        }

        private void Refresh()
        {
            _view.SetScore(_session.Score);
            _view.SetTime(_session.RemainingSeconds);
        }

        private void OnHit()
        {
            if (_hitTarget.Execute())
            {
                Refresh();
            }
        }
    }
}

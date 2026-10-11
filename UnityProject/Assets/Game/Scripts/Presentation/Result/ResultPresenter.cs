using System;
using Orquvia.Starter.Domain;
using VContainer.Unity;

namespace Orquvia.Starter.Presentation
{
    public sealed class ResultPresenter
        : IStartable, IDisposable
    {
        private readonly IReadOnlyGameSession _session;
        private readonly GameFlow _flow;
        private readonly ResultView _view;

        public ResultPresenter(
            IReadOnlyGameSession session,
            GameFlow flow,
            ResultView view)
        {
            _session = session;
            _flow = flow;
            _view = view;
        }

        public void Start()
        {
            _view.TitleButton.onClick.AddListener(OnClickTitle);
            _view.SetScore(_session.Score);
        }

        public void Dispose()
        {
            _view.TitleButton.onClick.RemoveListener(OnClickTitle);
        }

        private void OnClickTitle()
        {
            _flow.RequestTitle();
        }
    }
}

using System;
using VContainer.Unity;

namespace Orquvia.Starter.Presentation
{
    public sealed class TitlePresenter
        : IStartable, IDisposable
    {
        private readonly GameFlow _flow;
        private readonly TitleView _title;

        public TitlePresenter(
            GameFlow flow,
            TitleView title)
        {
            _flow = flow;
            _title = title;
        }

        public void Start()
        {
            _title.StartButton.onClick.AddListener(OnClickStart);
        }

        public void Dispose()
        {
            _title.StartButton.onClick.RemoveListener(OnClickStart);
        }

        private void OnClickStart() => _flow.RequestStartGame();
    }
}

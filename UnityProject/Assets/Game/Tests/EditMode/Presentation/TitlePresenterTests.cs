using NUnit.Framework;
using Orquvia.Starter.Presentation;
using Orquvia.Starter.Tests.Fakes;
using Orquvia.Starter.UseCase;
using UnityEngine;
using UnityEngine.UI;

namespace Orquvia.Starter.Tests.Presentation
{
    public sealed class TitlePresenterTests
    {
        private GameObject _root;
        private Button _startButton;
        private FakeSceneNavigator _navigator;
        private GameFlow _flow;
        private TitlePresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject(nameof(TitleView));
            var view = _root.AddComponent<TitleView>();
            _startButton = TestUi.CreateChild<Button>(_root, "Start");
            TestUi.Assign(view, "startButton", _startButton);

            _navigator = new FakeSceneNavigator();
            _flow = new GameFlow(_navigator, new FakeGameLogger());
            _presenter = new TitlePresenter(_flow, view);
        }

        [TearDown]
        public void TearDown()
        {
            _presenter.Dispose();
            _flow.Dispose();
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void ClickStart_RequestsGame()
        {
            _presenter.Start();

            _startButton.onClick.Invoke();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Game }));
        }

        [Test]
        public void ClickStart_AfterDispose_IsIgnored()
        {
            _presenter.Start();
            _presenter.Dispose();

            _startButton.onClick.Invoke();

            Assert.That(_navigator.Requests, Is.Empty);
        }
    }
}

using NUnit.Framework;
using Orquvia.Starter.Domain;
using Orquvia.Starter.Presentation;
using Orquvia.Starter.Tests.Fakes;
using Orquvia.Starter.UseCase;
using UnityEngine;
using UnityEngine.UI;

namespace Orquvia.Starter.Tests.Presentation
{
    public sealed class ResultPresenterTests
    {
        private GameObject _root;
        private Button _titleButton;
        private Text _scoreText;
        private GameSession _session;
        private FakeSceneNavigator _navigator;
        private GameFlow _flow;
        private ResultPresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject(nameof(ResultView));
            var view = _root.AddComponent<ResultView>();
            _titleButton = TestUi.CreateChild<Button>(_root, "Title");
            _scoreText = TestUi.CreateChild<Text>(_root, "Score");
            TestUi.Assign(view, "titleButton", _titleButton);
            TestUi.Assign(view, "scoreText", _scoreText);

            _session = new GameSession(new GameRules(10f, 5));
            _navigator = new FakeSceneNavigator();
            _flow = new GameFlow(_navigator, new FakeGameLogger());
            _presenter = new ResultPresenter(_session, _flow, view);
        }

        [TearDown]
        public void TearDown()
        {
            _presenter.Dispose();
            _flow.Dispose();
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void Start_ShowsFinalScore()
        {
            _session.Start();
            _session.TryHitTarget();
            _session.TryHitTarget();
            _session.TryHitTarget();
            _session.Advance(10f);

            _presenter.Start();

            Assert.That(_scoreText.text, Is.EqualTo("Score: 15"));
        }

        [Test]
        public void ClickTitle_RequestsTitle()
        {
            _presenter.Start();

            _titleButton.onClick.Invoke();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Title }));
        }

        [Test]
        public void ClickTitle_AfterDispose_IsIgnored()
        {
            _presenter.Start();
            _presenter.Dispose();

            _titleButton.onClick.Invoke();

            Assert.That(_navigator.Requests, Is.Empty);
        }
    }
}

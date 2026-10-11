using NUnit.Framework;
using Orquvia.Starter.Domain;
using Orquvia.Starter.Presentation;
using Orquvia.Starter.Tests.Fakes;
using Orquvia.Starter.UseCase;
using UnityEngine;
using UnityEngine.UI;

namespace Orquvia.Starter.Tests.Presentation
{
    public sealed class GamePresenterTests
    {
        private const float Duration = 10f;
        private const int PointsPerHit = 5;

        private GameObject _root;
        private Button _targetButton;
        private Text _scoreText;
        private Text _timeText;
        private GameSession _session;
        private FakeTimeProvider _time;
        private FakeSceneNavigator _navigator;
        private GameFlow _flow;
        private GamePresenter _presenter;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject(nameof(GameView));
            var view = _root.AddComponent<GameView>();
            _targetButton = TestUi.CreateChild<Button>(_root, "Target");
            _scoreText = TestUi.CreateChild<Text>(_root, "Score");
            _timeText = TestUi.CreateChild<Text>(_root, "Time");
            TestUi.Assign(view, "targetButton", _targetButton);
            TestUi.Assign(view, "scoreText", _scoreText);
            TestUi.Assign(view, "timeText", _timeText);

            _session = new GameSession(new GameRules(Duration, PointsPerHit));
            _time = new FakeTimeProvider();
            _navigator = new FakeSceneNavigator();
            _flow = new GameFlow(_navigator, new FakeGameLogger());
            _presenter = new GamePresenter(
                view,
                _session,
                new HitTargetUseCase(_session),
                new AdvanceGameUseCase(_session),
                new StartGameUseCase(_session),
                _flow,
                _time);
        }

        [TearDown]
        public void TearDown()
        {
            _presenter.Dispose();
            _flow.Dispose();
            Object.DestroyImmediate(_root);
        }

        [Test]
        public void Start_StartsSessionAndShowsInitialState()
        {
            _presenter.Start();

            Assert.That(_session.Status, Is.EqualTo(GameStatus.Running));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 0"));
            Assert.That(_timeText.text, Is.EqualTo("Time: 10"));
        }

        [Test]
        public void ClickTarget_AddsScoreAndRefreshes()
        {
            _presenter.Start();

            _targetButton.onClick.Invoke();
            _targetButton.onClick.Invoke();

            Assert.That(_session.Score, Is.EqualTo(PointsPerHit * 2));
            Assert.That(_scoreText.text, Is.EqualTo("Score: 10"));
        }

        [Test]
        public void ClickTarget_BeforeStart_IsNotHandled()
        {
            _targetButton.onClick.Invoke();

            Assert.That(_session.Score, Is.EqualTo(0));
        }

        [Test]
        public void Tick_AdvancesByProvidedDeltaTime()
        {
            _presenter.Start();
            _time.DeltaTime = 2.5f;

            _presenter.Tick();

            Assert.That(_session.RemainingSeconds, Is.EqualTo(7.5f));
            Assert.That(_timeText.text, Is.EqualTo("Time: 8"));
            Assert.That(_navigator.Requests, Is.Empty);
        }

        [Test]
        public void Tick_WhenTimeIsUp_RequestsResultAndDisablesTarget()
        {
            _presenter.Start();
            _time.DeltaTime = Duration;

            _presenter.Tick();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Result }));
            Assert.That(_targetButton.interactable, Is.False);
            Assert.That(_timeText.text, Is.EqualTo("Time: 0"));
        }

        [Test]
        public void Tick_AfterTimeIsUp_RequestsResultOnlyOnce()
        {
            _presenter.Start();
            _time.DeltaTime = Duration;

            _presenter.Tick();
            _presenter.Tick();
            _presenter.Tick();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Result }));
        }

        [Test]
        public void Dispose_UnsubscribesTarget()
        {
            _presenter.Start();
            _presenter.Dispose();

            _targetButton.onClick.Invoke();

            Assert.That(_session.Score, Is.EqualTo(0));
        }
    }
}

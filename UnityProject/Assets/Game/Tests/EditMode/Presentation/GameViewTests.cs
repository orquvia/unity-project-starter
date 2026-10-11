using NUnit.Framework;
using Orquvia.Starter.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Orquvia.Starter.Tests.Presentation
{
    public sealed class GameViewTests
    {
        private GameObject _root;
        private GameView _view;
        private Button _targetButton;
        private Text _scoreText;
        private Text _timeText;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject(nameof(GameView));
            _view = _root.AddComponent<GameView>();
            _targetButton = TestUi.CreateChild<Button>(_root, "Target");
            _scoreText = TestUi.CreateChild<Text>(_root, "Score");
            _timeText = TestUi.CreateChild<Text>(_root, "Time");
            TestUi.Assign(_view, "targetButton", _targetButton);
            TestUi.Assign(_view, "scoreText", _scoreText);
            TestUi.Assign(_view, "timeText", _timeText);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        // 残り時間は切り上げて表示する（0 秒になるまで "Time: 0" にならない）
        [TestCase(10f, "Time: 10")]
        [TestCase(9.5f, "Time: 10")]
        [TestCase(0.1f, "Time: 1")]
        [TestCase(0f, "Time: 0")]
        public void SetTime_RoundsUp(float seconds, string expected)
        {
            _view.SetTime(seconds);

            Assert.That(_timeText.text, Is.EqualTo(expected));
        }

        [Test]
        public void SetScore_FormatsScore()
        {
            _view.SetScore(120);

            Assert.That(_scoreText.text, Is.EqualTo("Score: 120"));
        }

        [Test]
        public void SetInteractable_TogglesTarget()
        {
            _view.SetInteractable(false);
            Assert.That(_targetButton.interactable, Is.False);

            _view.SetInteractable(true);
            Assert.That(_targetButton.interactable, Is.True);
        }
    }
}

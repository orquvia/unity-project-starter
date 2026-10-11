using System;
using NUnit.Framework;
using Orquvia.Starter.Domain;

namespace Orquvia.Starter.Tests.Domain
{
    public sealed class GameSessionTests
    {
        private const float Duration = 10f;
        private const int PointsPerHit = 5;

        private GameSession _session;

        [SetUp]
        public void SetUp()
        {
            _session = new GameSession(new GameRules(Duration, PointsPerHit));
        }

        // --- 初期状態 / Start ---

        [Test]
        public void Constructor_InitializesToReady()
        {
            Assert.That(_session.Score, Is.EqualTo(0));
            Assert.That(_session.RemainingSeconds, Is.EqualTo(Duration));
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Ready));
        }

        [Test]
        public void Start_SetsRunning()
        {
            _session.Start();

            Assert.That(_session.Status, Is.EqualTo(GameStatus.Running));
        }

        [Test]
        public void Start_AfterFinished_ResetsScoreAndTime()
        {
            _session.Start();
            _session.TryHitTarget();
            _session.Advance(Duration);

            _session.Start();

            Assert.That(_session.Score, Is.EqualTo(0));
            Assert.That(_session.RemainingSeconds, Is.EqualTo(Duration));
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Running));
        }

        // --- TryHitTarget ---

        [Test]
        public void TryHitTarget_WhileRunning_AddsPoints()
        {
            _session.Start();

            Assert.That(_session.TryHitTarget(), Is.True);
            Assert.That(_session.TryHitTarget(), Is.True);
            Assert.That(_session.Score, Is.EqualTo(PointsPerHit * 2));
        }

        [Test]
        public void TryHitTarget_WhileReady_IsIgnored()
        {
            Assert.That(_session.TryHitTarget(), Is.False);
            Assert.That(_session.Score, Is.EqualTo(0));
        }

        [Test]
        public void TryHitTarget_AfterFinished_IsIgnored()
        {
            _session.Start();
            _session.TryHitTarget();
            _session.Advance(Duration);

            Assert.That(_session.TryHitTarget(), Is.False);
            Assert.That(_session.Score, Is.EqualTo(PointsPerHit));
        }

        [Test]
        public void TryHitTarget_ScoreOverflow_ThrowsAndKeepsScore()
        {
            var session = new GameSession(new GameRules(Duration, int.MaxValue));
            session.Start();
            session.TryHitTarget();

            Assert.Throws<OverflowException>(() => session.TryHitTarget());
            Assert.That(session.Score, Is.EqualTo(int.MaxValue));
        }

        // --- Advance ---

        [TestCase(-0.1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Advance_InvalidDelta_ThrowsAndKeepsState(float deltaSeconds)
        {
            _session.Start();

            Assert.Throws<ArgumentOutOfRangeException>(() => _session.Advance(deltaSeconds));
            Assert.That(_session.RemainingSeconds, Is.EqualTo(Duration));
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Running));
        }

        [Test]
        public void Advance_InvalidDeltaWhileReady_Throws()
        {
            // 引数の検証は状態に関係なく行われる
            Assert.Throws<ArgumentOutOfRangeException>(() => _session.Advance(float.NaN));
        }

        [Test]
        public void Advance_WhileReady_DoesNothing()
        {
            Assert.That(_session.Advance(1f), Is.False);
            Assert.That(_session.RemainingSeconds, Is.EqualTo(Duration));
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Ready));
        }

        [Test]
        public void Advance_ZeroDelta_KeepsState()
        {
            _session.Start();

            Assert.That(_session.Advance(0f), Is.False);
            Assert.That(_session.RemainingSeconds, Is.EqualTo(Duration));
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Running));
        }

        [Test]
        public void Advance_LessThanRemaining_DecreasesTime()
        {
            _session.Start();

            Assert.That(_session.Advance(3f), Is.False);
            Assert.That(_session.RemainingSeconds, Is.EqualTo(Duration - 3f));
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Running));
        }

        [Test]
        public void Advance_ExactlyRemaining_Finishes()
        {
            _session.Start();

            Assert.That(_session.Advance(Duration), Is.True);
            Assert.That(_session.RemainingSeconds, Is.EqualTo(0f));
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Finished));
        }

        [Test]
        public void Advance_BeyondRemaining_ClampsToZero()
        {
            _session.Start();

            Assert.That(_session.Advance(Duration * 3f), Is.True);
            Assert.That(_session.RemainingSeconds, Is.EqualTo(0f));
        }

        [Test]
        public void Advance_AccumulatedDeltas_Finishes()
        {
            _session.Start();

            Assert.That(_session.Advance(4f), Is.False);
            Assert.That(_session.Advance(4f), Is.False);
            Assert.That(_session.Advance(4f), Is.True);
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Finished));
        }

        [Test]
        public void Advance_AfterFinished_ReportsFinishOnlyOnce()
        {
            _session.Start();
            _session.Advance(Duration);

            Assert.That(_session.Advance(1f), Is.False);
            Assert.That(_session.Status, Is.EqualTo(GameStatus.Finished));
        }
    }
}

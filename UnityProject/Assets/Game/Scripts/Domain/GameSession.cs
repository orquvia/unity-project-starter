using System;

namespace Orquvia.Starter.Domain
{
    public sealed class GameSession
        : IReadOnlyGameSession
    {
        private readonly GameRules _rules;

        public int Score { get; private set; }
        public float RemainingSeconds { get; private set; }
        public GameStatus Status { get; private set; }

        public GameSession(GameRules rules)
        {
            _rules = rules;
            Reset();
        }

        internal void Reset()
        {
            Score = 0;
            RemainingSeconds = _rules.DurationSeconds;
            Status = GameStatus.Ready;
        }

        internal void Start()
        {
            Reset();
            Status = GameStatus.Running;
        }

        internal bool TryHitTarget()
        {
            if (Status != GameStatus.Running)
            {
                return false;
            }

            Score = checked(Score + _rules.PointsPerHit);
            return true;
        }

        internal bool Advance(float deltaSeconds)
        {
            if (!float.IsFinite(deltaSeconds) || deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            if (Status != GameStatus.Running)
            {
                return false;
            }

            RemainingSeconds = Math.Max(0f, RemainingSeconds - deltaSeconds);

            if (RemainingSeconds > 0f)
            {
                return false;
            }

            Status = GameStatus.Finished;
            return true;
        }
    }
}

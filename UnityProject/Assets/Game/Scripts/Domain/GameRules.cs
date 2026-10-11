using System;

namespace Orquvia.Starter.Domain
{
    public enum GameStatus
    {
        Ready,
        Running,
        Finished
    }

    public sealed class GameRules
    {
        public float DurationSeconds { get; }
        public int PointsPerHit { get; }

        public GameRules(float durationSeconds, int pointsPerHit)
        {
            if (durationSeconds <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            }

            if (pointsPerHit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pointsPerHit));
            }

            DurationSeconds = durationSeconds;
            PointsPerHit = pointsPerHit;
        }
    }
}

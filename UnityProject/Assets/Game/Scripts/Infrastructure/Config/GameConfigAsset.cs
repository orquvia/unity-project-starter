using Orquvia.Starter.Domain;
using UnityEngine;

namespace Orquvia.Starter.Infrastructure
{
    [CreateAssetMenu(fileName = "GameConfigAsset", menuName = "Scriptable Objects/GameConfigAsset")]
    public sealed class GameConfigAsset : ScriptableObject
    {
        [SerializeField, Min(0.01f)] private float durationSeconds = 20f;
        [SerializeField, Min(1)] private int pointsPerHit = 10;

        public GameRules ToRules()
        {
            return new (durationSeconds, pointsPerHit);
        }
    }
}

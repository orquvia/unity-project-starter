using UnityEngine;

namespace Orquvia.Starter.Presentation
{
    public sealed class UnityTimeProvider : ITimeProvider
    {
        public float DeltaTime => Time.deltaTime;
    }
}

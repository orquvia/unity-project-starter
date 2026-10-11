using Orquvia.Starter.Presentation;

namespace Orquvia.Starter.Tests.Fakes
{
    public sealed class FakeTimeProvider : ITimeProvider
    {
        public float DeltaTime { get; set; }
    }
}

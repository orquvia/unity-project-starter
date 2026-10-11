using Orquvia.Starter.Domain;

namespace Orquvia.Starter.UseCase
{
    public sealed class AdvanceGameUseCase
    {
        private readonly GameSession _session;

        public AdvanceGameUseCase(GameSession session)
        {
            _session = session;
        }

        public bool Execute(float deltaSeconds)
        {
            return _session.Advance(deltaSeconds);
        }
    }
}

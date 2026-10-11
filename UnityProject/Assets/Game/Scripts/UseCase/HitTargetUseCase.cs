using Orquvia.Starter.Domain;

namespace Orquvia.Starter.UseCase
{
    public sealed class HitTargetUseCase
    {
        private readonly GameSession _session;

        public HitTargetUseCase(GameSession session)
        {
            _session = session;
        }

        public bool Execute()
        {
            return _session.TryHitTarget();
        }
    }
}

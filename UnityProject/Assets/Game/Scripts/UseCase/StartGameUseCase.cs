using Orquvia.Starter.Domain;

namespace Orquvia.Starter.UseCase
{
    public sealed class StartGameUseCase
    {
        private readonly GameSession _session;

        public StartGameUseCase(GameSession session)
        {
            _session = session;
        }

        public void Execute()
        {
            _session.Start();
        }    
    }
}

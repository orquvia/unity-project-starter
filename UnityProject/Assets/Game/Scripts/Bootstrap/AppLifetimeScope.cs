using Orquvia.Starter.Domain;
using Orquvia.Starter.UseCase;
using VContainer;
using VContainer.Unity;

namespace Orquvia.Starter.Bootstrap
{
    public sealed class AppLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            
            builder.Register<GameSession>(Lifetime.Singleton);
            builder.Register<StartGameUseCase>(Lifetime.Singleton);
            builder.Register<HitTargetUseCase>(Lifetime.Singleton);
            builder.Register<AdvanceGameUseCase>(Lifetime.Singleton);
        }
    }
}
using UnityEngine;
using Orquvia.Starter.Domain;
using Orquvia.Starter.UseCase;
using Orquvia.Starter.Infrastructure;
using VContainer;
using VContainer.Unity;
using Orquvia.Starter.Presentation;

namespace Orquvia.Starter.Bootstrap
{
    public sealed class AppLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfigAsset config;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(config.ToRules());
            builder.Register<GameSession>(Lifetime.Singleton)
                .AsSelf()
                .As<IReadOnlyGameSession>();
            builder.Register<StartGameUseCase>(Lifetime.Singleton);
            builder.Register<HitTargetUseCase>(Lifetime.Singleton);
            builder.Register<AdvanceGameUseCase>(Lifetime.Singleton);

            builder.Register<IGameLogger, UnityGameLogger>(Lifetime.Singleton);
            builder.Register<ISceneNavigator, UnitySceneNavigator>(Lifetime.Singleton);

            builder.Register<GameFlow>(Lifetime.Singleton);
        }
    }
}
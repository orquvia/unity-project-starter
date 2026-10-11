using UnityEngine;
using VContainer;
using VContainer.Unity;
using Orquvia.Starter.Presentation;

namespace Orquvia.Starter.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameView view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<GameView>(view);
            builder.RegisterEntryPoint<GamePresenter>();
        }
    }
}

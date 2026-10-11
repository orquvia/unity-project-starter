using UnityEngine;
using VContainer;
using VContainer.Unity;
using Orquvia.Starter.Presentation;

namespace Qrquvia.Starter.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameView view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<GameView>(view);
            builder.RegisterEntryPoint<GamePresenter>();
        }
    }
}

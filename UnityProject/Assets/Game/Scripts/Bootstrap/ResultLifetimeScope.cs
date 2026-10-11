using Orquvia.Starter.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Orquvia.Starter.Bootstrap
{
    public sealed class ResultLifetimeScope : LifetimeScope
    {
        [SerializeField] private ResultView view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<ResultView>(view);
            builder.RegisterEntryPoint<ResultPresenter>();
        }
    }
}

using UnityEngine;
using VContainer;
using VContainer.Unity;
using Orquvia.Starter.Presentation;

namespace Orquvia.Starter.Bootstrap
{
    public sealed class TitleLifetimeScope : LifetimeScope
    {
        [SerializeField] private TitleView titleView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<TitleView>(titleView);
            builder.RegisterEntryPoint<TitlePresenter>();
        }
    }
}

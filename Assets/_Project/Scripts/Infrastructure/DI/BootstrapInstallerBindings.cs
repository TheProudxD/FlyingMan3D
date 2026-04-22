using _Project.Scripts.Infrastructure.Services.Localization.UI;
using Reflex.Core;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class BootstrapInstallerBindings
    {
        public static void BindBootstrapScene(
            this ContainerBuilder builder,
            GameBootstraper gameBootstraper,
            LocalizedLabel[] localizedLabels)
        {
            builder.OnContainerBuilt += container =>
            {
                container.InjectAll(localizedLabels);
                gameBootstraper?.Activate();
            };
        }
    }
}

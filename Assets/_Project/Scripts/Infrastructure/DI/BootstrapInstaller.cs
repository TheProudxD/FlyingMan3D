using _Project.Scripts.Infrastructure.Services.Localization.UI;
using _Project.Scripts.Tools;
using _Project.Scripts.Tools.Extensions;
using Reflex.Core;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.DI
{
    public class BootstrapInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder) => builder.OnContainerBuilt += OnContainerBuilt;

        private void OnContainerBuilt(Container container)
        {
            InjectLocalizedLabel(container);
            FindAnyObjectByType<GameBootstraper>(FindObjectsInactive.Include).Activate();
        }

        private void InjectLocalizedLabel(Container container)
        {
            LocalizedLabel[] labels = FindObjectsOfType<LocalizedLabel>();

            foreach (LocalizedLabel label in labels)
            {
                container.Inject(label);
            }
        }
    }
}
using _Project.Scripts.Infrastructure.Services.Debug;
using _Project.Scripts.Infrastructure.Services.Localization.UI;
using _Project.Scripts.Tools;
using _Project.Scripts.Tools.Camera;
using Reflex.Core;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.DI
{
    public class MainSceneInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder) => builder.OnContainerBuilt += OnContainerBuilt;

        private void OnContainerBuilt(Container container)
        {
#if UNITY_EDITOR
            InjectDebug(container);
#endif
            InjectLocalizedLabel(container);
        }

        private void InjectDebug(Container container)
        {
            DebugController debugController = FindObjectOfType<DebugController>();

            if (debugController == null)
                return;

            container.Inject(debugController);
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
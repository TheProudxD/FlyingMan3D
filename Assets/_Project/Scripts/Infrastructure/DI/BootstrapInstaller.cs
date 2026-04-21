using _Project.Scripts.Infrastructure.Services.Localization.UI;
using _Project.Scripts.Tools;
using _Project.Scripts.Tools.Extensions;
using Reflex.Core;
using UnityEngine;
using _Project.Scripts.Infrastructure;

namespace _Project.Scripts.Infrastructure.DI
{
    public class BootstrapInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameBootstraper _gameBootstraper;
        [SerializeField] private LocalizedLabel[] _localizedLabels;

        public void InstallBindings(ContainerBuilder builder) => builder.OnContainerBuilt += OnContainerBuilt;

        private void OnContainerBuilt(Container container)
        {
            InjectLocalizedLabel(container);

            if (_gameBootstraper != null)
                _gameBootstraper.Activate();
        }

        private void InjectLocalizedLabel(Container container)
        {
            foreach (LocalizedLabel label in _localizedLabels)
            {
                if (label == null)
                    continue;

                container.Inject(label);
            }
        }

        private void OnValidate()
        {
            if (_gameBootstraper == null)
                _gameBootstraper = GetComponentInChildren<GameBootstraper>(true);

            _localizedLabels = GetComponentsInChildren<LocalizedLabel>(true);
        }
    }
}

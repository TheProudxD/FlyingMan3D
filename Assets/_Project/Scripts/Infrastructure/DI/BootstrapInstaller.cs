using _Project.Scripts.Infrastructure.Services.Localization.UI;
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

        public void InstallBindings(ContainerBuilder builder) =>
            builder.BindBootstrapScene(_gameBootstraper, _localizedLabels);

        private void OnValidate()
        {
            if (_gameBootstraper == null)
                _gameBootstraper = GetComponentInChildren<GameBootstraper>(true);

            _localizedLabels = GetComponentsInChildren<LocalizedLabel>(true);
        }
    }
}

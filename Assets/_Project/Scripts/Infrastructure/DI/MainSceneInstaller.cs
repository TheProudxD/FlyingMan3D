using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.Localization.UI;
using BhorGames.Mechanics;
using Reflex.Core;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.DI
{
    public class MainSceneInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private Indicator _indicator;
        [SerializeField] private Spawner _spawner;
        [SerializeField] private Platform _platform;
        [SerializeField] private LocalizedLabel[] _localizedLabels;

        public void InstallBindings(ContainerBuilder builder) =>
            builder.BindMainScene(_indicator, _spawner, _platform, _localizedLabels);

        private void OnValidate()
        {
            if (_indicator == null)
                _indicator = GetComponentInChildren<Indicator>(true);

            if (_spawner == null)
                _spawner = GetComponentInChildren<Spawner>(true);

            if (_platform == null)
                _platform = GetComponentInChildren<Platform>(true);

            _localizedLabels = GetComponentsInChildren<LocalizedLabel>(true);
        }
    }
}

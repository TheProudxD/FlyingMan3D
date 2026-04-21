using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Level;
using _Project.Scripts.Infrastructure.Services.Localization.UI;
using _Project.Scripts.Infrastructure.Services.Scene;
using _Project.Scripts.Tools;
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

        public void InstallBindings(ContainerBuilder builder)
        {
            var sceneRefs = new LevelSceneReferences(_indicator, _spawner, _platform);
            builder.AddSingleton(_ => sceneRefs);

            builder.OnContainerBuilt += container =>
            {
                var gameFactory = container.Resolve<GameFactory>();
                gameFactory.SetSceneRef(sceneRefs);

                var levelRuntimeObjectFactory = container.Resolve<LevelRuntimeObjectFactory>();
                levelRuntimeObjectFactory.SetSceneRef(sceneRefs);

                var levelPlayerLifecycleService = container.Resolve<LevelPlayerLifecycleService>();
                levelPlayerLifecycleService.SetSceneRef(sceneRefs);

                OnContainerBuilt(container);
            };
        }

        private void OnContainerBuilt(Container container)
        {
            container.Inject(_indicator);
            container.Inject(_spawner);
            container.Inject(_platform);
            InjectLocalizedLabel(container);
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

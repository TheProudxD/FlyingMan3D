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
            LevelSceneReferences sceneRefs = new LevelSceneReferences(_indicator, _spawner, _platform);
            builder.AddSingleton(_ => sceneRefs);
            builder.OnContainerBuilt += container =>
            {
                container.Resolve<GameFactory>().SetSceneRef(sceneRefs);
                container.Resolve<LevelRuntimeObjectFactory>().SetSceneRef(sceneRefs);
                container.Resolve<LevelPlayerLifecycleService>().SetSceneRef(sceneRefs);
                InjectSceneComponents(container);
            };
        }

        private void InjectSceneComponents(Container container)
        {
            InjectIfAssigned(container, _indicator);
            InjectIfAssigned(container, _spawner);
            InjectIfAssigned(container, _platform);
            InjectLocalizedLabels(container);
        }

        private void InjectLocalizedLabels(Container container)
        {
            foreach (LocalizedLabel label in _localizedLabels)
            {
                if (label == null)
                    continue;

                container.Inject(label);
            }
        }

        private static void InjectIfAssigned(Container container, Object target)
        {
            if (target != null)
                container.Inject(target);
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

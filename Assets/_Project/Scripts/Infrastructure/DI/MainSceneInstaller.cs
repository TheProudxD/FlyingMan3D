using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.Debug;
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

        public void InstallBindings(ContainerBuilder builder)
        {
            var sceneRefs = new LevelSceneReferences(_indicator, _spawner, _platform);
            builder.AddSingleton(_ => sceneRefs);

            builder.OnContainerBuilt += container =>
            {
                var gameFactory = container.Resolve<GameFactory>();
                gameFactory.SetSceneRef(sceneRefs);

                var levelLifecycleService = container.Resolve<LevelLifecycleService>();
                levelLifecycleService.SetSceneRef(sceneRefs);

                OnContainerBuilt(container);
            };
        }

        private void OnContainerBuilt(Container container)
        {
#if UNITY_EDITOR
            InjectDebug(container);
#endif

            container.Inject(_indicator);
            container.Inject(_spawner);
            container.Inject(_platform);
            InjectLocalizedLabel(container);
        }

        private void InjectDebug(Container container)
        {
            DebugController debugController = FindAnyObjectByType<DebugController>();

            if (debugController == null)
                return;

            container.Inject(debugController);
        }

        private void InjectLocalizedLabel(Container container)
        {
            LocalizedLabel[] labels = FindObjectsByType<LocalizedLabel>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (LocalizedLabel label in labels)
            {
                container.Inject(label);
            }
        }
    }
}

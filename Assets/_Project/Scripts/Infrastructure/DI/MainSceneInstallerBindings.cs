using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Level;
using _Project.Scripts.Infrastructure.Services.Localization.UI;
using _Project.Scripts.Infrastructure.Services.Scene;
using BhorGames.Mechanics;
using Reflex.Core;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class MainSceneInstallerBindings
    {
        public static void BindMainScene(
            this ContainerBuilder builder,
            Indicator indicator,
            Spawner spawner,
            Platform platform,
            LocalizedLabel[] localizedLabels)
        {
            LevelSceneReferences sceneReferences = new LevelSceneReferences(indicator, spawner, platform);
            builder.AddSingleton(_ => sceneReferences);

            builder.OnContainerBuilt += container =>
            {
                container.Resolve<GameFactory>().SetSceneRef(sceneReferences);
                container.Resolve<LevelRuntimeObjectFactory>().SetSceneRef(sceneReferences);
                container.Resolve<LevelPlayerLifecycleService>().SetSceneRef(sceneReferences);

                container.InjectIfAssigned(indicator);
                container.InjectIfAssigned(spawner);
                container.InjectIfAssigned(platform);
                container.InjectAll(localizedLabels);
            };
        }
    }
}

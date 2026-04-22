using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Camera;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Infrastructure.Services.Logger;
using _Project.Scripts.Infrastructure.Timer;
using _Project.Scripts.Tools.Camera;
using _Project.Scripts.UI;
using Reflex.Core;
using ILogger = _Project.Scripts.Infrastructure.Services.Logger.ILogger;
using GameTimer = _Project.Scripts.Infrastructure.Timer.Timer;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class ProjectInstallerCoreBindings
    {
        public static void BindProjectCore(
            this ContainerBuilder builder,
            CameraSetup cameraSetupPrefab,
            LoadingCurtain loadingCurtainPrefab)
        {
            builder.AddSingleton<ILogger>(_ => new NoLogger(true));
            builder.AddSingleton(c => new GameTimer(0));

            builder.AddSingleton(typeof(ProjectObjectInjector));
            builder.AddSingleton(typeof(AssetProvider));
            builder.AddSingleton(typeof(SpriteAtlasLoader));
            builder.AddSingleton(typeof(ConfigService));
            builder.AddSingleton(typeof(LocalizationService));
            builder.AddSingleton(typeof(DeviceSpecificGraphics));
            builder.AddSingleton(typeof(InputReader));

            builder.AddSingleton(c =>
                c.Resolve<AssetProvider>().Instantiate<LoadingCurtain>(loadingCurtainPrefab.gameObject));

            builder.AddSingleton(c =>
                c.Resolve<AssetProvider>().Instantiate<CameraSetup>(cameraSetupPrefab.gameObject));

            builder.AddSingleton(typeof(CameraService));

            builder.OnContainerBuilt += container =>
            {
                container.Resolve<ProjectObjectInjector>().SetContainer(container);
                container.Resolve<AssetProvider>().SetConfigService(container.Resolve<ConfigService>());
            };
        }
    }
}

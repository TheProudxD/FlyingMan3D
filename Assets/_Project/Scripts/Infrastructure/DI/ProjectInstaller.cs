using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Infrastructure.Services.Logger;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Infrastructure.Services.Review;
using Reflex.Core;
using UnityEngine;
using ILogger = _Project.Scripts.Infrastructure.Services.Logger.ILogger;

namespace _Project.Scripts.Infrastructure.DI
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            BindLogger(builder);
            BindAds(builder);
            BindMetric(builder);
            BindReview(builder);
            BindTimer(builder);
            BindSaveService(builder);
            BindStatistics(builder);
            BindLeaderboard(builder);
            BindConfigs(builder);
            BindDeviceGraphics(builder);
            BindLocalization(builder);
            BindLoadingBar(builder);
            BindCamera(builder);
            BindInput(builder);
            BindFactories(builder);
            BindAudio(builder);
            BindWindows(builder);
            BindStates(builder);
            BindAssets(builder);
            BindGameplayServices(builder);

            builder.OnContainerBuilt += c => c.Resolve<ConfigService>();
        }

        private void BindLogger(ContainerBuilder builder) =>
            builder.AddSingleton<ILogger>(_ => new NoLogger(true));

        private void BindAds(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(AdsService));

        private void BindMetric(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(MetricService));

        private void BindTimer(ContainerBuilder builder) => builder.AddSingleton(c => new Timer(0));

        private void BindSaveService(ContainerBuilder builder)
        {
            builder.AddSingleton<IPersistentProgressService>(_ => new PersistentProgressService());
            builder.AddSingleton(typeof(SaveLoadService));
        }

        private void BindStatistics(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(StatisticsService));

        private void BindLeaderboard(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(LeaderboardService));

        private void BindReview(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(ReviewShowService));

        private void BindLocalization(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(LocalizationService));

        private void BindConfigs(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(ConfigService));

        private void BindDeviceGraphics(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(DeviceSpecificGraphics));

        private void BindWindows(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(WindowService));

        private void BindInput(ContainerBuilder builder) =>
            builder.AddSingleton(typeof(InputReader));

        private void BindGameplayServices(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(LevelResourceService));
            builder.AddSingleton(typeof(MoneyResourceService));
            builder.AddSingleton(typeof(AnimationService));
            builder.AddSingleton(typeof(SceneLoader));
            builder.AddSingleton(typeof(HeartTracker));

            builder.AddSingleton(c => new StateMachine(
                c.Resolve<BootstrapState>(),
                c.Resolve<LoadLevelState>(),
                c.Resolve<LoadProgressState>(),
                c.Resolve<GameLoopState>(),
                c.Resolve<WinLevelState>(), c.Resolve<LoseLevelState>(), c.Resolve<RestartLevelState>(),
                c.Resolve<ReplayLevelState>(), c.Resolve<ContinueLevelState>()));
        }

        private void BindAudio(ContainerBuilder builder) => builder.AddSingleton(typeof(AudioService));

        private void BindFactories(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(UIFactory));
            builder.AddSingleton(typeof(GameFactory));
        }

        private void BindStates(ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(BootstrapState));
            builder.AddSingleton(typeof(LoadLevelState));
            builder.AddSingleton(typeof(LoadProgressState));
            builder.AddSingleton(typeof(GameLoopState));
            builder.AddSingleton(typeof(WinLevelState));
            builder.AddSingleton(typeof(LoseLevelState));
            builder.AddSingleton(typeof(ContinueLevelState));
            builder.AddSingleton(typeof(RestartLevelState));
            builder.AddSingleton(typeof(ReplayLevelState));
        }

        private void BindAssets(ContainerBuilder builder) => builder.AddSingleton(typeof(AssetProvider));

        private void BindCamera(ContainerBuilder builder)
        {
            builder.AddSingleton(c =>
            {
                var asset = c.Resolve<AssetProvider>();
                return asset.CreateCameraSetup().MainCamera;
            });
        }

        private void BindLoadingBar(ContainerBuilder builder) =>
            builder.AddSingleton(c => c.Resolve<AssetProvider>().CreateLoadingCurtain());
    }
}
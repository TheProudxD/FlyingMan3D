using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Level;
using _Project.Scripts.Infrastructure.Services.Resources;
using Reflex.Core;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class ProjectInstallerGameplayBindings
    {
        public static void BindProjectGameplay(this ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(PlayerFactory));
            builder.AddSingleton(typeof(EnemyFactory));
            builder.AddSingleton(typeof(FxFactory));
            builder.AddSingleton(typeof(GameFactory));

            builder.AddSingleton(typeof(PlayerFinishTransitionService));
            builder.AddSingleton(typeof(PlayerFinishCombatService));
            builder.AddSingleton(typeof(LevelEntityRegistry));
            builder.AddSingleton(typeof(PlayerStateCopyService));
            builder.AddSingleton(typeof(LevelRuntimeObjectFactory));
            builder.AddSingleton(typeof(LevelPlayerLifecycleService));
            builder.AddSingleton(typeof(LevelResourceService));
            builder.AddSingleton(typeof(MoneyResourceService));
            builder.AddSingleton(typeof(AnimationService));
            builder.AddSingleton(typeof(SceneLoader));

            builder.AddSingleton(typeof(BootstrapState));
            builder.AddSingleton(typeof(LoadLevelState));
            builder.AddSingleton(typeof(LoadGameState));
            builder.AddSingleton(typeof(GameLoopState));
            builder.AddSingleton(typeof(WinLevelState));
            builder.AddSingleton(typeof(LoseLevelState));
            builder.AddSingleton(typeof(ContinueLevelState));
            builder.AddSingleton(typeof(RestartLevelState));
            builder.AddSingleton(typeof(ReplayLevelState));

            builder.AddSingleton(container =>
            {
                var states = new IExitableState[]
                {
                    container.Resolve<BootstrapState>(),
                    container.Resolve<LoadLevelState>(),
                    container.Resolve<LoadGameState>(),
                    container.Resolve<GameLoopState>(),
                    container.Resolve<WinLevelState>(),
                    container.Resolve<LoseLevelState>(),
                    container.Resolve<RestartLevelState>(),
                    container.Resolve<ReplayLevelState>(),
                    container.Resolve<ContinueLevelState>()
                };

                return new StateMachine(states);
            });
        }
    }
}

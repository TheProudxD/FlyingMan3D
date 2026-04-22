using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using Reflex.Core;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class ProjectInstallerProgressBindings
    {
        public static void BindProjectProgress(this ContainerBuilder builder)
        {
            builder.AddSingleton<IPersistentProgressService>(_ => new PersistentProgressService());
            builder.AddSingleton(typeof(SaveLoadService));
            builder.AddSingleton(typeof(StatisticsService));
            builder.AddSingleton(typeof(LeaderboardService));
        }
    }
}

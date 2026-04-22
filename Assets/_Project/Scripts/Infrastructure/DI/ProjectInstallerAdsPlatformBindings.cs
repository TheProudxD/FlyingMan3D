using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Review;
using Reflex.Core;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class ProjectInstallerAdsPlatformBindings
    {
        public static void BindProjectAdsAndPlatform(this ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(AdsService));
            builder.AddSingleton(typeof(MetricService));
            builder.AddSingleton(typeof(ReviewShowService));
        }
    }
}

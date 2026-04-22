using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Windows;
using Reflex.Core;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class ProjectInstallerUiBindings
    {
        public static void BindProjectUi(this ContainerBuilder builder)
        {
            builder.AddSingleton(typeof(AudioClipLoader));
            builder.AddSingleton(typeof(AudioService));

            builder.AddSingleton(typeof(WindowRegistry));
            builder.AddSingleton(typeof(WindowResourceManager));
            builder.AddSingleton(typeof(WindowService));

            builder.AddSingleton(typeof(UIFactory));
        }
    }
}

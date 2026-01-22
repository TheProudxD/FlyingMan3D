using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Infrastructure.Services.Windows
{
    public class WindowResourceManager : IService
    {
        private readonly ConfigService _configService;

        public WindowResourceManager(ConfigService configService) => _configService = configService;

        public void ReleaseAsset(WindowId windowId)
        {
            var windowConfig = _configService.ForWindow(windowId);
            windowConfig?.Prefab.ReleaseAsset();
        }
    }
}
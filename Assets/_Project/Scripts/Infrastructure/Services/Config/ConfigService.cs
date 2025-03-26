using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Infrastructure.Services.Config
{
    public class ConfigService : IService, IInitializable
    {
        private readonly AssetProvider _assetProvider;
        private ConfigContainer _configContainer;
        private Dictionary<WindowId, WindowConfig> _windowConfigs;

        public ConfigService(AssetProvider assetProvider) => _assetProvider = assetProvider;

        public T Get<T>() where T : Config => _configContainer.Configs.First(c => c is T) as T;

        public WindowConfig ForWindow(WindowId window) => _windowConfigs.GetValueOrDefault(window);

        public IEnumerator Initialize()
        {
            _configContainer = _assetProvider.GetConfigContainer();

            _windowConfigs = _assetProvider.GetWindowStaticData().Configs
                .ToDictionary(x => x.Id, x => x);

            yield break;
        }
    }
}
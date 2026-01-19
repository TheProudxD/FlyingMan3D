using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.Services.Config
{
    public class ConfigService : IService, ITaskInitializable
    {
        private readonly AssetProvider _assetProvider;
        private ConfigContainer _configContainer;
        private Dictionary<WindowId, WindowConfig> _windowConfigs;

        public ConfigService(AssetProvider assetProvider) => _assetProvider = assetProvider;

        public T Get<T>() where T : Config => _configContainer.Configs.First(c => c is T) as T;

        public WindowConfig ForWindow(WindowId window) => _windowConfigs.GetValueOrDefault(window);

        public async UniTask Initialize()
        {
            _configContainer = await _assetProvider.GetConfigContainer();

            _windowConfigs = (await _assetProvider.GetWindowStaticData()).Configs
                .ToDictionary(x => x.Id, x => x);
        }
    }
}
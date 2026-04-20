using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Windows;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Views;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class UIFactory : IService, ITaskInitializable
    {
        private readonly AssetProvider _assetProvider;
        private readonly WindowRegistry _windowRegistry;

        private Transform _uiRoot;
        private UIFactory _iuiFactoryImplementation;
        private Hud _hud;

        public UIFactory(AssetProvider assetProvider, WindowRegistry windowRegistry)
        {
            _assetProvider = assetProvider;
            _windowRegistry = windowRegistry;
        }

        public async UniTask Initialize()
        {
            await RegisterWindowFactories();
        }

        public Transform GetUIRoot() => _uiRoot;

        public Hud GetHUD() => _hud;

        public void RegisterHud(Hud hud) => _hud = hud;
        
        private async UniTask RegisterWindowFactories()
        {
            _uiRoot = (await _assetProvider.CreateUIRoot()).transform;
            _windowRegistry.RegisterFactory(WindowId.Pause, () => InstantiateRegistered(WindowId.Pause, _uiRoot));
            _windowRegistry.RegisterFactory(WindowId.Lose, () => InstantiateRegistered(WindowId.Lose, _uiRoot));
            _windowRegistry.RegisterFactory(WindowId.Tutorial, () => InstantiateRegistered(WindowId.Tutorial, _uiRoot));
            _windowRegistry.RegisterFactory(WindowId.Leaderboard, () => InstantiateRegistered(WindowId.Leaderboard, _uiRoot));
            _windowRegistry.RegisterFactory(WindowId.Win, () => InstantiateRegistered(WindowId.Win, _uiRoot));
            _windowRegistry.RegisterFactory(WindowId.HUD, () => InstantiateRegistered(WindowId.HUD, _uiRoot));
        }

        private UniTask<UIContainer> InstantiateRegistered(WindowId windowId, Transform parent) =>
            _assetProvider.Instantiate(windowId, parent);
    }
}

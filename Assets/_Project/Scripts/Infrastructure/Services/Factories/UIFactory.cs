using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Views;
using _Project.Scripts.UI.Windows;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class UIFactory : IService
    {
        private readonly AssetProvider _assetProvider;

        private Transform _uiRoot;
        private UIFactory _iuiFactoryImplementation;
        private Hud _hud;

        public UIFactory(AssetProvider assetProvider) => _assetProvider = assetProvider;

        public async Task Initialize(WindowService windowService)
        {
            _uiRoot = (await _assetProvider.CreateUIRoot()).transform;
            _hud = (Hud)await windowService.Show(WindowId.HUD);
            _hud.Initialize();
        }

        public Transform GetUIRoot() => _uiRoot;

        public Hud GetHUD() => _hud;

        public async Task<UIContainer> CreatePauseWindow() => await InstantiateRegistered(WindowId.Pause, _uiRoot);

        public async Task<UIContainer> CreateLoseWindow() => await InstantiateRegistered(WindowId.Lose, _uiRoot);

        public async Task<UIContainer> CreateTutorialWindow() =>
            await InstantiateRegistered(WindowId.Tutorial, _uiRoot);

        public async Task<UIContainer> CreateLeaderboardWindow() =>
            await InstantiateRegistered(WindowId.Leaderboard, _uiRoot);

        public async Task<UIContainer> CreateWinWindow() => await InstantiateRegistered(WindowId.Win, _uiRoot);

        public async Task<UIContainer> CreateHUD() => await InstantiateRegistered(WindowId.HUD, _uiRoot);

        private Task<UIContainer> InstantiateRegistered(WindowId windowId, Transform parent) =>
            _assetProvider.Instantiate(windowId, parent);
    }
}
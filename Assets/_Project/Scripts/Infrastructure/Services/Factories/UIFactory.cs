using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Views;
using _Project.Scripts.UI.Windows;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class UIFactory : IInitializable
    {
        private readonly AssetProvider _assetProvider;
        private readonly GameFactory _gameFactory;

        private Transform _uiRoot;
        private UIFactory _iuiFactoryImplementation;
        private Hud _hud;

        public UIFactory(AssetProvider assetProvider, GameFactory gameFactory)
        {
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
        }

        public IEnumerator Initialize()
        {
            _uiRoot = _assetProvider.CreateUIRoot();
            _hud = _assetProvider.CreateHUD();
            yield break;
        }

        public Transform GetUIRoot() => _uiRoot;

        public Hud GetHUD() => _hud;

        public WindowBase CreatePauseWindow() => InstantiateRegistered(WindowId.Pause, _uiRoot);

        public WindowBase CreateLoseWindow() => InstantiateRegistered(WindowId.Lose, _uiRoot);

        //public WindowBase CreateTutorialWindow() => InstantiateRegistered(WindowId.Tutorial, _uiRoot);

        public WindowBase CreateLeaderboardWindow() => InstantiateRegistered(WindowId.Leaderboard, _uiRoot);

        public WindowBase CreateWinWindow() => InstantiateRegistered(WindowId.Win, _uiRoot);

        public void ShowMinusTimer() => Object.FindObjectOfType<TimerView>().ShowMinusText();

        private WindowBase InstantiateRegistered(WindowId windowId, Transform parent) =>
            _assetProvider.Instantiate(windowId, parent);
    }
}
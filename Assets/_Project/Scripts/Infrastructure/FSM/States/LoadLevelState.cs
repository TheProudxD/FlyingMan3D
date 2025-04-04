using System.Collections;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools.Coroutine;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Views;
using Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoadLevelState : IPayLoadState<string>, IInitializable
    {
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SaveLoadService _saveLoadService;
        private readonly UIFactory _uiFactory;
        private readonly HeartTracker _heartTracker;
        private readonly AssetProvider _assetProvider;
        private readonly StatisticsService _statisticsService;
        private readonly GameFactory _gameFactory;
        private readonly LevelResourceService _levelResourceService;

        private StateMachine _stateMachine;
        private string _sceneName;

        public LoadLevelState(SceneLoader sceneLoader, LoadingCurtain loadingCurtain,
            SaveLoadService saveLoadService, UIFactory uiFactory, HeartTracker heartTracker,
            AssetProvider assetProvider, GameFactory gameFactory, StatisticsService statisticsService,
            LevelResourceService levelResourceService)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _heartTracker = heartTracker;
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
            _statisticsService = statisticsService;
            _levelResourceService = levelResourceService;
        }

        public void Enter(string sceneName = null)
        {
            _loadingCurtain.Show();

            _sceneName = sceneName;

            Coroutines.StartRoutine(Initialize());
        }

        public IEnumerator Initialize()
        {
            yield return _uiFactory.Initialize();
            yield return _gameFactory.Initialize();

            Object.FindObjectOfType<Indicator>().Enable();
            PlayerController player = _gameFactory.CreatePlayer();
            Hud hud = _uiFactory.GetHUD();
            hud.Initialize();
            yield return _gameFactory.GetSpawner().Initialize();

            foreach (ScoreBaseView view in Object.FindObjectsByType<ScoreBaseView>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                view.Initialize();
            }

            hud.Show();

            CinemachineVirtualCamera camera = _gameFactory.GetFinishCamera();
            camera.Priority = 5;
            TryShowTutorial();
            _gameFactory.GetScore().Reset();
            _statisticsService.IncreaseGamesPlayedNumberCounter();
            _levelResourceService.Current.Value = _levelResourceService.ObservableValue.Value;
            _gameFactory.CreateLevel(_levelResourceService.Current.Value);
            player.Initialize();
            _loadingCurtain.Hide();
            _sceneLoader.Load(_sceneName, OnLoaded);
        }

        private void OnLoaded()
        {
            _saveLoadService.InformAll();

            _stateMachine.Enter<GameLoopState>();
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        private void TryShowTutorial()
        {
            /*
            if (_levelResourceService.ObservableValue.Value == 1)
            {
                _windowService.Show(WindowId.Tutorial);
            }*/
        }


        public void Exit()
        {
            //_loadingCurtain.Hide();
        }
    }
}
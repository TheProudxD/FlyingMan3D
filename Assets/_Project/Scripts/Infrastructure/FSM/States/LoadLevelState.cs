using System;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Infrastructure.Services.Windows;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Scripts.Gameplay;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoadLevelState : IState
    {
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SaveLoadService _saveLoadService;
        private readonly UIFactory _uiFactory;
        private readonly StatisticsService _statisticsService;
        private readonly GameFactory _gameFactory;
        private readonly LevelResourceService _levelResourceService;
        private readonly WindowService _windowService;
        private readonly MetricService _metricService;
        private readonly PlayerFactory _playerFactory;

        private StateMachine _stateMachine;
        private string _sceneName;

        public LoadLevelState(LoadingCurtain loadingCurtain,
            SaveLoadService saveLoadService, GameFactory gameFactory, StatisticsService statisticsService,
            LevelResourceService levelResourceService, WindowService windowService, UIFactory uiFactory,
            MetricService metricService, PlayerFactory playerFactory)
        {
            _loadingCurtain = loadingCurtain;
            _saveLoadService = saveLoadService;
            _gameFactory = gameFactory;
            _statisticsService = statisticsService;
            _levelResourceService = levelResourceService;
            _windowService = windowService;
            _uiFactory = uiFactory;
            _metricService = metricService;
            _playerFactory = playerFactory;
        }

        public void Enter()
        {
            _loadingCurtain.Show();

            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            try
            {
                _gameFactory.ClearLevelHolder();
                await _gameFactory.CreateSlingshot(new Vector3(0, 4.5f, 0));
                PlayerController player = await _playerFactory.CreateMainPlayer();
                await _gameFactory.GetSpawner().Initialize();

                Hud hud = _uiFactory.GetHUD();
                hud.Show();
                hud.ActivateStartText();

                TryShowTutorialAsync().Forget();
                _statisticsService.IncreaseGamesPlayedNumberCounter();
                _levelResourceService.Current.Value = _levelResourceService.ObservableValue.Value;
                _gameFactory.CreateLevel();
                player.Initialize();
                _gameFactory.GetIndicator().Enable();
                _gameFactory.SetPlayerCamera();

                _metricService.LevelStarted(_levelResourceService.Current.Value);
                _loadingCurtain.Hide();
                _stateMachine.Enter<GameLoopState, IExitableState>(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        private async UniTask TryShowTutorialAsync()
        {
            try
            {
                if (_levelResourceService.ObservableValue.Value == 1)
                {
                    await _windowService.Show(WindowId.Tutorial);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void Exit()
        {
            //_loadingCurtain.Hide();
        }
    }
}

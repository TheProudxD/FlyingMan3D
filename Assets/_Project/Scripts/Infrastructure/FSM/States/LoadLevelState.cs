using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using UnityEngine;

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

        private StateMachine _stateMachine;
        private string _sceneName;

        public LoadLevelState(LoadingCurtain loadingCurtain,
            SaveLoadService saveLoadService, GameFactory gameFactory, StatisticsService statisticsService,
            LevelResourceService levelResourceService, WindowService windowService, UIFactory uiFactory,
            MetricService metricService)
        {
            _loadingCurtain = loadingCurtain;
            _saveLoadService = saveLoadService;
            _gameFactory = gameFactory;
            _statisticsService = statisticsService;
            _levelResourceService = levelResourceService;
            _windowService = windowService;
            _uiFactory = uiFactory;
            _metricService = metricService;
        }

        public void Enter()
        {
            _loadingCurtain.Show();

            Initialize();
        }

        public void Initialize()
        {
            _gameFactory.ClearLevelHolder();
            _gameFactory.CreateSlingshot(new Vector3(0, 4.5f, 0));
            PlayerController player = _gameFactory.CreateMainPlayer();
            _gameFactory.GetSpawner().Initialize();

            Hud hud = _uiFactory.GetHUD();
            hud.Show();
            hud.ActivateStartText();

            TryShowTutorial();
            _statisticsService.IncreaseGamesPlayedNumberCounter();
            _levelResourceService.Current.Value = _levelResourceService.ObservableValue.Value;
            _gameFactory.CreateLevel();
            player.Initialize();
            _gameFactory.GetIndicator().Enable();
            _gameFactory.SetPlayerCamera();

            _metricService.LevelStarted(_levelResourceService.Current.Value);
            _saveLoadService.InformAll();
            _loadingCurtain.Hide();
            _stateMachine.Enter<GameLoopState, IExitableState>(this);
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        private void TryShowTutorial()
        {
            if (_levelResourceService.ObservableValue.Value == 1)
            {
                _windowService.Show(WindowId.Tutorial);
            }
        }


        public void Exit()
        {
            //_loadingCurtain.Hide();
        }
    }
}
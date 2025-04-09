using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.UI;
using Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class ReplayLevelState : IState
    {
        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly AudioService _audioService;
        private readonly StatisticsService _statisticsService;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly LevelResourceService _levelResourceService;

        private StateMachine _stateMachine;

        public ReplayLevelState(GameFactory gameFactory,
            AudioService audioService, UIFactory uiFactory, StatisticsService statisticsService,
            LoadingCurtain loadingCurtain,
            LevelResourceService levelResourceService)
        {
            _gameFactory = gameFactory;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _statisticsService = statisticsService;
            _loadingCurtain = loadingCurtain;
            _levelResourceService = levelResourceService;
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Enter()
        {
            _levelResourceService.Current.Value = _levelResourceService.ObservableValue.Value - 1;


            _gameFactory.ClearLevelHolder();
            _gameFactory.CreateSlingshot(new Vector3(0, 4.5f, 0));
            PlayerController player = _gameFactory.CreateMainPlayer();
            _gameFactory.GetSpawner().Initialize();

            Hud hud = _uiFactory.GetHUD();
            hud.Show();
            hud.ActivateStartText();

            CinemachineVirtualCamera camera = _gameFactory.GetFinishCamera();
            camera.Priority = 5;
            _gameFactory.GetScore().Reset();
            _statisticsService.IncreaseGamesPlayedNumberCounter();
            _gameFactory.CreateLevel();
            player.Initialize();
            _gameFactory.GetIndicator().Enable();
            _uiFactory.GetHUD().Show();
            _loadingCurtain.Hide();

            _stateMachine.Enter<GameLoopState, IExitableState>(this);
        }

        public void Exit() { }
    }
}
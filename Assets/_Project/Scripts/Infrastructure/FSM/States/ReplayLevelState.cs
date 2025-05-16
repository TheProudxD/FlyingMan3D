using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.UI;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class ReplayLevelState : IState
    {
        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly StatisticsService _statisticsService;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly LevelResourceService _levelResourceService;

        private StateMachine _stateMachine;

        public ReplayLevelState(GameFactory gameFactory, UIFactory uiFactory, StatisticsService statisticsService,
            LoadingCurtain loadingCurtain,
            LevelResourceService levelResourceService)
        {
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
            _statisticsService = statisticsService;
            _loadingCurtain = loadingCurtain;
            _levelResourceService = levelResourceService;
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public async void Enter()
        {
            _levelResourceService.Current.Value = _levelResourceService.ObservableValue.Value - 1;
            
            _gameFactory.ClearLevelHolder();
            await _gameFactory.CreateSlingshot(new Vector3(0, 4.5f, 0));
            PlayerController player = await _gameFactory.CreateMainPlayer();
            await _gameFactory.GetSpawner().Initialize();

            Hud hud = _uiFactory.GetHUD();
            hud.Show();
            hud.ActivateStartText();

            _gameFactory.SetPlayerCamera();
            _statisticsService.IncreaseGamesPlayedNumberCounter();
            _gameFactory.CreateLevel();
            player.Initialize();
            _gameFactory.GetIndicator().Enable();
            _loadingCurtain.Hide();

            _stateMachine.Enter<GameLoopState, IExitableState>(this);
        }

        public void Exit() { }
    }
}
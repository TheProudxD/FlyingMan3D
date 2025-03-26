using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public enum GameEnterState
    {
        Continue,
        LoadNext,
        Restart, // in game (not win or already win)
        Replay //after winning
    }

    public class GameLoopState : IPayLoadState<GameEnterState>
    {
        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly AudioService _audioService;
        private readonly StatisticsService _statisticsService;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly Timer _timer;
        private readonly LevelResourceService _levelResourceService;

        private StateMachine _stateMachine;
        private Hud _hud;
        private Level _level;

        public GameLoopState(GameFactory gameFactory,
            AudioService audioService, UIFactory uiFactory, StatisticsService statisticsService,
            LoadingCurtain loadingCurtain, Timer timer,
            LevelResourceService levelResourceService)
        {
            _gameFactory = gameFactory;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _statisticsService = statisticsService;
            _loadingCurtain = loadingCurtain;
            _timer = timer;
            _levelResourceService = levelResourceService;
        }

        public GameEnterState GameEnterState { get; private set; }

        public void Enter(GameEnterState enterState)
        {
            Resources.UnloadUnusedAssets();
            GameEnterState = enterState;

            switch (GameEnterState)
            {
                case GameEnterState.Continue:
                    TryShowTutorial();
                    int additionalTime = 60;
                    _timer.Start(additionalTime);
                    break;
                case GameEnterState.LoadNext:
                    DestroyCurrentLevel();
                    TryShowTutorial();
                    ResetScore();
                    _statisticsService.IncreaseGamesPlayedNumberCounter();
                    _levelResourceService.Current.Value = _levelResourceService.ObservableValue.Value;
                    CreateLevel();
                    break;
                case GameEnterState.Restart:
                    DestroyCurrentLevel();
                    ResetScore();
                    _statisticsService.IncreaseGamesPlayedNumberCounter();
                    CreateLevel();
                    break;
                case GameEnterState.Replay:
                    DestroyCurrentLevel();
                    ResetScore();
                    _statisticsService.IncreaseGamesPlayedNumberCounter();
                    _levelResourceService.Current.Value = _levelResourceService.ObservableValue.Value - 1;
                    CreateLevel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(enterState), enterState, null);
            }

            // _gameFactory.GetInputService().Enable();
            CreateHUD();
            _loadingCurtain.Hide();
        }

        private void CreateLevel()
        {
            _level = _gameFactory.CreateLevel(_levelResourceService.Current.Value);
            _timer.Start(_level.LevelTimer);
        }

        private void DestroyCurrentLevel()
        {
            if (_level != null) Object.Destroy(_level.gameObject);
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Exit()
        {
            // _gameFactory.GetInputService().Disable();
            DisableHud();
        }

        private void CreateHUD()
        {
            _hud = _uiFactory.GetHUD();
            EnableHud();
        }

        private void TryShowTutorial()
        {
            /*
            if (_levelResourceService.ObservableValue.Value == 1)
            {
                _windowService.Show(WindowId.Tutorial);
            }*/
        }

        private void ResetScore()
        {
            _gameFactory.GetScore().Reset();
            _gameFactory.GetScore().Reset();
        }

        private void EnableHud() => _hud.Show();

        private void DisableHud() => _hud.Hide();
    }
}
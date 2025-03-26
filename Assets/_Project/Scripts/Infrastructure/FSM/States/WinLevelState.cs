using System;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class WinLevelState : IState
    {
        private readonly WindowService _windowService;
        private readonly Timer _timer;
        private readonly GameFactory _gameFactory;
        private readonly LeaderboardService _leaderboardService;
        private readonly LevelResourceService _levelResourceService;
        private readonly AudioService _audioService;
        private readonly GameLoopState _gameLoopState;

        public WinLevelState(WindowService windowService, Timer timer, GameFactory gameFactory,
            LeaderboardService leaderboardService, LevelResourceService levelResourceService, AudioService audioService,
            GameLoopState gameLoopState)
        {
            _windowService = windowService;
            _timer = timer;
            _gameFactory = gameFactory;
            _leaderboardService = leaderboardService;
            _levelResourceService = levelResourceService;
            _audioService = audioService;
            _gameLoopState = gameLoopState;
        }

        public void Enter()
        {
            _gameFactory.players = null;
            _audioService.PlayWinSound();
            _timer.Stop();
            SetRecordInLeaderboard();
            _windowService.Show(WindowId.Win);

            switch (_gameLoopState.GameEnterState)
            {
                case GameEnterState.LoadNext or GameEnterState.Restart or GameEnterState.Continue:
                    _levelResourceService.Increase(this);
                    break;
                case GameEnterState.Replay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_gameLoopState.GameEnterState),
                        _gameLoopState.GameEnterState, null);
            }
        }

        public void SetStateMachine(StateMachine value)
        {
            //_stateMachine = value;
        }

        public void Exit() { }

        private void SetRecordInLeaderboard()
        {
            int current = _gameFactory.GetScore().Value.Value;

            if (current > _leaderboardService.GetMaxLeaderboardScore())
            {
                _leaderboardService.SetMaxLeaderboardScore(current);
            }
        }
    }
}
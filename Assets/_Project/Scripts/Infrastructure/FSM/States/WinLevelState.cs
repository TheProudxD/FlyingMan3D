using System.Collections;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Infrastructure.Services.Review;
using _Project.Scripts.Infrastructure.Services.Windows;
using _Project.Scripts.Tools.Coroutine;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using Object = UnityEngine.Object;
using _Project.Scripts.Gameplay;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class WinLevelState : IState
    {
        private readonly WindowService _windowService;
        private readonly GameFactory _gameFactory;
        private readonly PlayerFactory _playerFactory;
        private readonly LeaderboardService _leaderboardService;
        private readonly LevelResourceService _levelResourceService;
        private readonly AudioService _audioService;
        private readonly GameLoopState _gameLoopState;
        private readonly ReviewShowService _reviewShowService;

        public WinLevelState(WindowService windowService, GameFactory gameFactory,
            LeaderboardService leaderboardService, LevelResourceService levelResourceService, AudioService audioService,
            GameLoopState gameLoopState, ReviewShowService reviewShowService, PlayerFactory playerFactory)
        {
            _windowService = windowService;
            _gameFactory = gameFactory;
            _leaderboardService = leaderboardService;
            _levelResourceService = levelResourceService;
            _audioService = audioService;
            _gameLoopState = gameLoopState;
            _reviewShowService = reviewShowService;
            _playerFactory = playerFactory;
        }

        public void Enter() => Coroutines.StartRoutine(WinCoroutine());

        private IEnumerator WinCoroutine()
        {
            ParticleSystem winParticle = _gameFactory.GetSpawner().WinParticle;
            PlayerController player = _playerFactory.GetMainPlayer();
            if (player != null)
                winParticle.transform.position = player.transform.position;
            winParticle.Play();
            yield return new WaitForSeconds(2);

            _playerFactory.DestroyPlayers();
            _audioService.PlayWinSound();
            SetRecordInLeaderboard();
            yield return _windowService.Show(WindowId.Win);
            IExitableState state = _gameLoopState.FromState;

            if (state is RestartLevelState or ContinueLevelState or LoadLevelState)
            {
                _levelResourceService.Increase(this);
            }

            if (_levelResourceService.Current.Value > 3 && _reviewShowService.CanShow())
            {
                _reviewShowService.Show();
            }
        }

        public void SetStateMachine(StateMachine value)
        {
            //_stateMachine = value;
        }

        public void Exit() { }

        private void SetRecordInLeaderboard()
        {
            int current = _levelResourceService.Current.Value;

            if (current > _leaderboardService.GetMaxLeaderboardScore())
            {
                _leaderboardService.SetMaxLeaderboardScore(current);
            }
        }
    }
}

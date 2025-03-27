using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoseLevelState : IState
    {
        private readonly Timer _timer;
        private readonly WindowService _windowService;
        private readonly HintService _hintService;
        private readonly AudioService _audioService;
        private readonly GameFactory _gameFactory;

        public LoseLevelState(WindowService windowService, Timer timer, HintService hintService, AudioService audioService, GameFactory gameFactory)
        {
            _windowService = windowService;
            _timer = timer;
            _hintService = hintService;
            _audioService = audioService;
            _gameFactory = gameFactory;
        }

        public void Enter()
        {
            _gameFactory.DestroyPlayers();
            LoseLevel();
        }

        public void SetStateMachine(StateMachine value) { }

        public void Exit() { }

        private void LoseLevel()
        {
            _audioService.PlayLoseSound();
            _hintService.DisableHintMode();
            _timer.Stop();
            _windowService.Show(WindowId.Lose);
        }
    }
}
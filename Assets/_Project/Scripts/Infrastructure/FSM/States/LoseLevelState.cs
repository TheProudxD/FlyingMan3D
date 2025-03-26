using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoseLevelState : IState
    {
        private readonly Timer _timer;
        private readonly WindowService _windowService;
        private readonly HintService _hintService;
        private readonly AudioService _audioService;

        public LoseLevelState(WindowService windowService, Timer timer, HintService hintService, AudioService audioService)
        {
            _windowService = windowService;
            _timer = timer;
            _hintService = hintService;
            _audioService = audioService;
        }

        public void Enter()
        {
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
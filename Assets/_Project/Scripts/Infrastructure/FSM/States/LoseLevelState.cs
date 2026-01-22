using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoseLevelState : IState
    {
        private readonly Timer _timer;
        private readonly WindowService _windowService;
        private readonly AudioService _audioService;
        private readonly PlayerFactory _playerFactory;

        public LoseLevelState(WindowService windowService, Timer timer, AudioService audioService, PlayerFactory playerFactory)
        {
            _windowService = windowService;
            _timer = timer;
            _audioService = audioService;
            _playerFactory = playerFactory;
        }

        public void Enter()
        {
            _playerFactory.DestroyPlayers();
            LoseLevelAsync().Forget();
        }

        public void SetStateMachine(StateMachine value) { }

        public void Exit() { }

        private async UniTask LoseLevelAsync()
        {
            _audioService.PlayLoseSound();
            _timer.Stop();
            await _windowService.Show(WindowId.Lose);
        }
    }
}
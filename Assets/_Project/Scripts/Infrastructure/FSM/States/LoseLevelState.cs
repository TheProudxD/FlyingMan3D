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
        private readonly GameFactory _gameFactory;

        public LoseLevelState(WindowService windowService, Timer timer, AudioService audioService, GameFactory gameFactory)
        {
            _windowService = windowService;
            _timer = timer;
            _audioService = audioService;
            _gameFactory = gameFactory;
        }

        public void Enter()
        {
            _gameFactory.DestroyPlayers();
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
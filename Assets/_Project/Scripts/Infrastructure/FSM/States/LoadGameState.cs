using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Windows;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Views;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoadGameState : IState
    {
        private readonly SaveLoadService _saveLoadService;
        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly WindowService _windowService;

        private StateMachine _stateMachine;

        public LoadGameState(SaveLoadService saveLoadService, GameFactory gameFactory, UIFactory uiFactory,
            WindowService windowService)
        {
            _saveLoadService = saveLoadService;
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
            _windowService = windowService;
        }

        public void Enter() => LoadProgressAsync().Forget();

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Exit() { }

        private async UniTask LoadProgressAsync()
        {
            _saveLoadService.LoadProgress();

            await _gameFactory.Initialize();
            await _uiFactory.Initialize();

            // Initialize HUD
            var hudContainer = await _windowService.Show(WindowId.HUD);
            var hud = hudContainer as Hud;
            hud?.Initialize();
            _uiFactory.RegisterHud(hud);

            if (hud != null)
            {
                foreach (ScoreBaseView view in hud.GetComponentsInChildren<ScoreBaseView>(true))
                {
                    view.Initialize();
                }
            }

            _stateMachine.Enter<LoadLevelState>();
        }
    }
}

using System.Collections;
using _Project.Scripts.Data;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Tools.Coroutine;
using _Project.Scripts.UI.Views;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoadGameState : IState
    {
        private readonly IPersistentProgressService _progressService;
        private readonly SaveLoadService _saveLoadService;
        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly WindowService _windowService;

        private StateMachine _stateMachine;

        public LoadGameState(IPersistentProgressService progressService, SaveLoadService saveLoadService,
            GameFactory gameFactory, UIFactory uiFactory, WindowService windowService)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
            _windowService = windowService;
        }

        public void Enter() => LoadProgress();

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Exit() { }

        private async void LoadProgress()
        {
            var progress = _saveLoadService.LoadProgress();
            _progressService.Progress = progress.Item1;
            _progressService.PowerupProgress = progress.Item2;

            await _gameFactory.Initialize();
            await _uiFactory.Initialize(_windowService);

            foreach (ScoreBaseView view in Object.FindObjectsByType<ScoreBaseView>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                view.Initialize();
            }

            _stateMachine.Enter<LoadLevelState>();
        }
    }
}
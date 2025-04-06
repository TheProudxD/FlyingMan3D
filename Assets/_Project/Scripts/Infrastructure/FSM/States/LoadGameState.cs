using System.Collections;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Tools.Coroutine;
using _Project.Scripts.UI;
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

        private StateMachine _stateMachine;

        public LoadGameState(IPersistentProgressService progressService, SaveLoadService saveLoadService,
            GameFactory gameFactory, UIFactory uiFactory)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
        }

        public void Enter() => Coroutines.StartRoutine(LoadProgress());

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Exit() { }

        private IEnumerator LoadProgress()
        {
            _progressService.Progress = _saveLoadService.LoadProgress();

            yield return _gameFactory.Initialize();
            yield return _uiFactory.Initialize();

            foreach (ScoreBaseView view in Object.FindObjectsByType<ScoreBaseView>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                view.Initialize();
            }

            _stateMachine.Enter<LoadLevelState>();
        }
    }
}
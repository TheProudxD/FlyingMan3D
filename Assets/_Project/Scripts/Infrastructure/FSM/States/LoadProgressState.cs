using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.UI;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoadProgressState : IState
    {
        private readonly IPersistentProgressService _progressService;
        private readonly SaveLoadService _saveLoadService;

        private StateMachine _stateMachine;

        public LoadProgressState(IPersistentProgressService progressService, SaveLoadService saveLoadService)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
        }

        public void Enter() => LoadProgressOrInitNew();

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Exit() { }

        private void LoadProgressOrInitNew()
        {
            _progressService.Progress = _saveLoadService.LoadProgress();
            _stateMachine.Enter<LoadLevelState, string>(SceneNames.MAIN_SCENE);
        }
    }
}
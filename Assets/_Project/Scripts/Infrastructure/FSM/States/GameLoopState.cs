using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class GameLoopState : IPayLoadState<IExitableState>
    {
        private static readonly int Win = Animator.StringToHash("Win");

        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;

        private StateMachine _stateMachine;
        private Hud _hud;
        private Level _level;

        public GameLoopState(GameFactory gameFactory, UIFactory uiFactory)
        {
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
        }

        public IExitableState FromState { get; set; }

        public void Enter(IExitableState fromState)
        {
            Resources.UnloadUnusedAssets();
            FromState = fromState;
            _hud = _uiFactory.GetHUD();
            _gameFactory.EnemiesCounter.Changed += CheckEntitiesCount;
            _gameFactory.PlayersCounter.Changed += CheckEntitiesCount;
            // _gameFactory.GetInputService().Enable();
        }

        private void CheckEntitiesCount(int count)
        {
            if (_gameFactory.Players.Count == 0 && _gameFactory.Enemies.Count >= 0)
            {
                foreach (Enemy item in _gameFactory.Enemies)
                {
                    if (item == null)
                        continue;

                    item.Animator?.SetBool(Win, true);
                    Object.Destroy(item.GetComponent<Rigidbody>());
                }

                _stateMachine.Enter<LoseLevelState>();
            }
            else if (_gameFactory.Enemies.Count == 0 &&
                     _gameFactory.Players.Count > 0 &&
                     _gameFactory.GetFinish() != null &&
                     _gameFactory.GetIndicator().Enabled == false)
            {
                foreach (PlayerController item in _gameFactory.Players)
                {
                    if (item == null)
                        continue;

                    item.Animator?.SetBool(Win, true);
                    Object.Destroy(item.GetComponent<Rigidbody>());
                }

                _stateMachine.Enter<WinLevelState>();
            }
        }

        public void Exit()
        {
            _gameFactory.EnemiesCounter.Changed -= CheckEntitiesCount;
            _gameFactory.PlayersCounter.Changed -= CheckEntitiesCount;
            _hud.Hide();
            // _gameFactory.GetInputService().Disable();
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;
    }
}
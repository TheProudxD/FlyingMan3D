using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using Object = UnityEngine.Object;
using _Project.Scripts.Gameplay;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class GameLoopState : IPayLoadState<IExitableState>
    {
        private static readonly int Win = Animator.StringToHash("Win");

        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly PlayerFactory _playerFactory;
        private readonly EnemyFactory _enemyFactory;

        private StateMachine _stateMachine;
        private Hud _hud;
        private Level _level;

        public GameLoopState(GameFactory gameFactory, UIFactory uiFactory, PlayerFactory playerFactory, EnemyFactory enemyFactory)
        {
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
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
            var players = _playerFactory.GetAllPlayers();
            var enemies = _enemyFactory.GetAllEnemies();
            if (players.Count == 0 && enemies.Count >= 0)
            {
                foreach (EnemyBase item in enemies)
                {
                    if (item == null)
                        continue;

                    item.Animator?.SetBool(Win, true);
                    Object.Destroy(item.GetComponent<Rigidbody>());
                }

                _stateMachine.Enter<LoseLevelState>();
            }
            else if (enemies.Count == 0 &&
                     players.Count > 0 &&
                     _gameFactory.GetFinish() != null &&
                     !_gameFactory.GetIndicator().Enabled)
            {
                foreach (PlayerController item in players)
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

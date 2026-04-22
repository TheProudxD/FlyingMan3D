using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class ContinueLevelState : IState
    {
        private static readonly int Win = Animator.StringToHash("Win");

        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly PlayerFactory _playerFactory;
        private readonly EnemyFactory _enemyFactory;

        private StateMachine _stateMachine;

        public ContinueLevelState(GameFactory gameFactory, UIFactory uiFactory, PlayerFactory playerFactory,
            EnemyFactory enemyFactory)
        {
            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
            _playerFactory = playerFactory;
            _enemyFactory = enemyFactory;
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Enter() => EnterAsync().Forget();

        private async UniTask EnterAsync()
        {
            ResetEnemiesWinState();

            PlayerController player = await _playerFactory.CreateMainPlayer();
            Hud hud = _uiFactory.GetHUD();
            hud?.Show();
            hud?.ActivateStartText();

            player.Initialize();
            _gameFactory.SetPlayerCamera();
            _gameFactory.GetIndicator().Enable();

            _stateMachine.Enter<GameLoopState, IExitableState>(this);
        }

        private void ResetEnemiesWinState()
        {
            foreach (EnemyBase enemy in _enemyFactory.GetAllEnemies())
            {
                if (enemy == null)
                    continue;

                enemy.Animator?.SetBool(Win, false);
            }
        }

        public void Exit() { }
    }
}

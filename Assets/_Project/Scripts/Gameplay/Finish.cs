using System.Collections;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class Finish : MonoBehaviour
    {
        [Inject] private StateMachine _stateMachine;
        [Inject] private GameFactory _gameFactory;
        [Inject] private EnemyFactory _enemyFactory;
        [Inject] private AudioService _audioService;
        [Inject] private PlayerFinishTransitionService _playerFinishTransitionService;

        private bool _attack;
        private bool _isGameOver;
        private WaitForSeconds _waiter;

        private void Start()
        {
            _waiter = new WaitForSeconds(1.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.CompareTag("Player") == false)
                return;

            bool shouldStartCombat = !_attack;

            if (shouldStartCombat)
            {
                _audioService.PlayHitSound();

                _attack = true;
                _gameFactory.SetFinishCamera(transform.position.z);

                foreach (EnemyBase enemy in _enemyFactory.GetAllEnemies())
                {
                    if (enemy != null && enemy.gameObject.activeInHierarchy)
                        enemy.Initialize();
                }
            }

            StartCoroutine(SetAttackState(other, shouldStartCombat));
        }

        private IEnumerator SetAttackState(Collider other, bool shouldPlayTransitionSlowMotion)
        {
            Transform root = other.gameObject.transform.root;

            if (!root.TryGetComponent(out PlayerController playerController))
                yield break;

            _playerFinishTransitionService.BeginTransition(playerController);

            if (shouldPlayTransitionSlowMotion)
            {
                Time.timeScale = 0.5f;
                yield return _waiter;
                Time.timeScale = 1;
            }

            if (root == null || root.gameObject == null)
                yield break;

            PlayerFinishMover playerFinishMover = _playerFinishTransitionService.CompleteTransition(playerController);
            if (playerFinishMover == null)
                yield break;

            playerFinishMover.Initialize();
        }
    }
}

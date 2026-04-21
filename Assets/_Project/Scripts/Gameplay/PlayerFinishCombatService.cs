using Cysharp.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class PlayerFinishCombatService : IService
    {
        private readonly EnemyFactory _enemyFactory;
        private readonly AudioService _audioService;
        private readonly FxFactory _fxFactory;

        public PlayerFinishCombatService(EnemyFactory enemyFactory, AudioService audioService, FxFactory fxFactory)
        {
            _enemyFactory = enemyFactory;
            _audioService = audioService;
            _fxFactory = fxFactory;
        }

        public async UniTask<PlayerFinishCollisionResult> ResolveCollision(
            PlayerController playerController,
            Transform ownerTransform,
            Collision collision,
            int health,
            int damage,
            bool canSmoke)
        {
            int nextHealth = health;
            bool nextCanSmoke = canSmoke;

            if (collision.transform.root.TryGetComponent(out EnemyBase enemy) && !enemy.IsDie && !playerController.IsDie)
            {
                _audioService.PlayHitSound();

                if (canSmoke)
                {
                    nextCanSmoke = false;
                    await _fxFactory.CreateSmoke(
                        new Vector3(0f, 2f, ownerTransform.position.z),
                        Quaternion.Euler(-90f, 0f, 0f)
                    );
                }

                enemy.TakeDamage(damage);
                nextHealth--;

                if (nextHealth <= 0)
                {
                    _audioService.PlayDieSound();
                    playerController.Die();
                }
            }

            bool shouldEnableMovement =
                ownerTransform != null &&
                ownerTransform.root != null &&
                ownerTransform.root.gameObject != null &&
                !ownerTransform.root.gameObject.CompareTag("Enemy") &&
                collision.gameObject.CompareTag("Platform");

            return new PlayerFinishCollisionResult(nextHealth, nextCanSmoke, shouldEnableMovement);
        }

        public GameObject GetNearestTarget(Vector3 position)
        {
            var enemies = _enemyFactory.GetAllEnemies();
            if (enemies == null || enemies.Count == 0)
                return null;

            float minDistance = float.MaxValue;
            int index = 0;

            for (int i = 1; i < enemies.Count; i++)
            {
                float distance = Vector3.Distance(enemies[i].transform.position, position);

                if (minDistance <= distance)
                    continue;

                minDistance = distance;
                index = i;
            }

            return enemies[index]?.gameObject;
        }
    }

    public readonly struct PlayerFinishCollisionResult
    {
        public PlayerFinishCollisionResult(int health, bool canSmoke, bool shouldEnableMovement)
        {
            Health = health;
            CanSmoke = canSmoke;
            ShouldEnableMovement = shouldEnableMovement;
        }

        public int Health { get; }
        public bool CanSmoke { get; }
        public bool ShouldEnableMovement { get; }
    }
}

using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.Level;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class EnemyFactory : IService
    {
        private readonly LevelLifecycleService _levelLifecycle;

        public EnemyFactory(LevelLifecycleService levelLifecycle)
        {
            _levelLifecycle = levelLifecycle;
        }

        public async UniTask CreateEnemy(EnemyType enemyType, Vector3 position, float rotation)
        {
            await _levelLifecycle.CreateEnemy(enemyType, position, rotation);
        }

        public IReadOnlyList<EnemyBase> GetAllEnemies() => _levelLifecycle.Enemies;

        public void RemoveEnemy(EnemyBase enemy) => _levelLifecycle.RemoveEnemy(enemy);
    }
}
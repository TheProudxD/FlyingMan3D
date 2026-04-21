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
        private readonly LevelRuntimeObjectFactory _levelRuntimeObjectFactory;
        private readonly LevelEntityRegistry _levelEntityRegistry;

        public EnemyFactory(
            LevelRuntimeObjectFactory levelRuntimeObjectFactory,
            LevelEntityRegistry levelEntityRegistry)
        {
            _levelRuntimeObjectFactory = levelRuntimeObjectFactory;
            _levelEntityRegistry = levelEntityRegistry;
        }

        public async UniTask CreateEnemy(EnemyType enemyType, Vector3 position, float rotation)
        {
            await _levelRuntimeObjectFactory.CreateEnemy(enemyType, position, rotation);
        }

        public IReadOnlyList<EnemyBase> GetAllEnemies() => _levelEntityRegistry.Enemies;

        public void RemoveEnemy(EnemyBase enemy) => _levelEntityRegistry.RemoveEnemy(enemy);
    }
}

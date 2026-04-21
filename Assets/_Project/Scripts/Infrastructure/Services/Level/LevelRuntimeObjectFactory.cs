using System;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Scene;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Level
{
    public class LevelRuntimeObjectFactory : IService
    {
        private readonly AssetProvider _assetProvider;
        private readonly LevelEntityRegistry _entityRegistry;

        private LevelSceneReferences _sceneRefs;
        private Finish _finish;

        public LevelRuntimeObjectFactory(AssetProvider assetProvider, LevelEntityRegistry entityRegistry)
        {
            _assetProvider = assetProvider;
            _entityRegistry = entityRegistry;
        }

        public void SetSceneRef(LevelSceneReferences sceneRefs) => _sceneRefs = sceneRefs;

        public async UniTask<EnemyBase> CreateEnemy(EnemyType enemyType, Vector3 position, float rotation)
        {
            EnemyBase enemy = enemyType switch
            {
                EnemyType.Simple or EnemyType.WithGun or EnemyType.WithGunAndShield =>
                    await _assetProvider.CreateSimpleEnemy(position, Quaternion.Euler(0f, 180f, 0f)),
                EnemyType.Big => await _assetProvider.CreateBigEnemy(position, Quaternion.Euler(0f, 180f, 0f)),
                EnemyType.Large => await _assetProvider.CreateLargeEnemy(position, Quaternion.Euler(0f, 180f, 0f)),
                _ => throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null)
            };

            enemy.transform.Rotate(0, rotation, 0);
            enemy.transform.Translate(new Vector3(0, 0, -16f));

            _entityRegistry.AddEnemy(enemy);
            _entityRegistry.TrackLevelObject(enemy.gameObject);
            return enemy;
        }

        public async UniTask<Finish> CreateFinish(Vector3 position, Quaternion rotation)
        {
            _finish = await _assetProvider.CreateFinish(position, rotation);
            _entityRegistry.TrackLevelObject(_finish.gameObject);
            return _finish;
        }

        public Finish GetFinish() => _finish;

        public async UniTask<RingHolder> CreateRing(Vector3 position, Spawner.Colors[] colorArray, int index)
        {
            RingHolder ringHolder = await _assetProvider.CreateRing(position, colorArray, index);
            _entityRegistry.TrackLevelObject(ringHolder.gameObject);
            return ringHolder;
        }

        public async UniTask CreateBarrel(Vector3 randomPosition)
        {
            ExplosionBarrel barrel = await _assetProvider.CreateBarrel(randomPosition);
            _entityRegistry.TrackLevelObject(barrel.gameObject);
        }

        public async UniTask CreateSlingshot(Vector3 position)
        {
            GameObject slingshotGo = await _assetProvider.CreateSlingshot(position);
            _entityRegistry.TrackLevelObject(slingshotGo);

            if (_sceneRefs != null)
                _sceneRefs.Slingshot = slingshotGo.GetComponent<Slingshot>();
        }

        public void ResetState() => _finish = null;
    }
}

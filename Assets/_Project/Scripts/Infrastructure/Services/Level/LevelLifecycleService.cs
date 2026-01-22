using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Scene;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services.Level
{
    public class LevelLifecycleService : IService, ITaskInitializable
    {
        private readonly AssetProvider _assetProvider;

        private readonly List<GameObject> _levelHolder = new();
        private readonly List<PlayerController> _players = new();
        private readonly List<EnemyBase> _enemies = new();

        private LevelSceneReferences _sceneRefs;
        private Finish _finish;

        public IReadOnlyList<PlayerController> Players => _players;
        public IReadOnlyList<EnemyBase> Enemies => _enemies;
        public ObservableVariable<int> EnemiesCounter { get; private set; }
        public ObservableVariable<int> PlayersCounter { get; private set; }

        public LevelLifecycleService(AssetProvider assetProvider) => _assetProvider = assetProvider;

        public void SetSceneRef(LevelSceneReferences sceneRefs) => _sceneRefs = sceneRefs;

        public UniTask Initialize()
        {
            EnemiesCounter = new ObservableVariable<int>(Enemies.Count);
            PlayersCounter = new ObservableVariable<int>(Players.Count);
            return UniTask.CompletedTask;
        }
        
        public async UniTask<PlayerController> CreateMainPlayer()
        {
            var startPosition = new Vector3(0, 1.75f, -1);
            var mainPlayer = await _assetProvider.CreatePlayer(startPosition);
            Rigidbody capsule = _sceneRefs.Slingshot.Capsule;
            var fixedJoint = mainPlayer.SelfHips.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = capsule;
            mainPlayer.SetInitial(fixedJoint, capsule.transform);
            AddPlayer(mainPlayer);

            _levelHolder.Add(mainPlayer.gameObject);
            return mainPlayer;
        }

        public PlayerController GetNewPlayer()
        {
            float spawnGap = UnityEngine.Random.Range(2f, 5f);
            Vector3 randomPos = UnityEngine.Random.onUnitSphere * spawnGap;

            GameObject root = GetMainPlayer().gameObject;

            PlayerController player =
                _assetProvider.CreatePlayer(root, root.transform.position + randomPos, root.transform.rotation);

            player.Initialize();
            CopyTransformData(root.transform, player.transform);
            AddPlayer(player);
            return player;
        }

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
            AddEnemy(enemy);
            _levelHolder.Add(enemy.gameObject);
            return enemy;
        }

        public async UniTask<Finish> CreateFinish(Vector3 vector3, Quaternion identity)
        {
            _finish = await _assetProvider.CreateFinish(vector3, identity);
            _levelHolder.Add(_finish.gameObject);
            return _finish;
        }
        
        public Finish GetFinish() => _finish;

        public async UniTask<RingHolder> CreateRing(Vector3 calculateRingPosition, Spawner.Colors[] colorArray, int index)
        {
            RingHolder ringHolder = await _assetProvider.CreateRing(calculateRingPosition, colorArray, index);
            _levelHolder.Add(ringHolder.gameObject);
            return ringHolder;
        }

        public async UniTask CreateBarrel(Vector3 randomPosition)
        {
            ExplosionBarrel barrel = await _assetProvider.CreateBarrel(randomPosition);
            _levelHolder.Add(barrel.gameObject);
        }

        public async UniTask CreateSlingshot(Vector3 position)
        {
            GameObject slingshotGo = await _assetProvider.CreateSlingshot(position);
            _levelHolder.Add(slingshotGo);
            _sceneRefs.Slingshot = slingshotGo.GetComponent<Slingshot>();
        }

        public void AddPlayer(PlayerController player)
        {
            _players.Add(player);
            PlayersCounter.Value = _players.Count;
        }

        public void AddEnemy(EnemyBase enemy)
        {
            _enemies.Add(enemy);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void RemovePlayer(PlayerController player)
        {
            _players.Remove(player);
            PlayersCounter.Value = _players.Count;

            if (player != null)
                Object.Destroy(player.gameObject);
        }

        public void RemoveEnemy(EnemyBase enemy)
        {
            _enemies.Remove(enemy);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void DestroyPlayers()
        {
            foreach (PlayerController player in Players)
            {
                if (player == null)
                    continue;

                Object.Destroy(player.gameObject);
            }

            _players.Clear();
        }

        public PlayerController GetMainPlayer() => _players.Count > 0 ? _players[0] : null;

        public void DestroyLastPlayer()
        {
            int playersCount = _players.Count - 1;
            PlayerController playerController = _players[playersCount];
            Object.Destroy(playerController.gameObject);
            RemovePlayer(playerController);
        }

        public void ClearLevelHolder()
        {
            if (_levelHolder == null || _levelHolder.Count == 0)
                return;

            foreach (GameObject gameObject in _levelHolder)
            {
                Object.Destroy(gameObject);
            }

            foreach (PlayerController player in _players)
            {
                Object.Destroy(player?.gameObject);
            }

            foreach (EnemyBase enemy in _enemies)
            {
                Object.Destroy(enemy?.gameObject);
            }

            _levelHolder.Clear();
            _players.Clear();
            _enemies.Clear();
        }

        private void CopyTransformData(Transform sourceTransform, Transform targetTransform)
        {
            if (sourceTransform.childCount != targetTransform.childCount)
            {
                UnityEngine.Debug.LogError("Players have different hierarchies!");
            }

            for (int i = 0; i < sourceTransform.childCount; i++)
            {
                Transform source = sourceTransform.GetChild(i);
                Transform target = targetTransform.GetChild(i);

                var rb = target.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = source.GetComponent<Rigidbody>().linearVelocity;
                }

                CopyTransformData(source, target);
            }
        }
    }
}
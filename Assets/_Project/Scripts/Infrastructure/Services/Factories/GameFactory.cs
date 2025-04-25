using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools.Camera;
using BhorGames.Mechanics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class GameFactory : IService, IInitializable
    {
        private readonly LevelResourceService _levelResourceService;
        private readonly SaveLoadService _saveLoadService;
        private readonly LeaderboardService _leaderboardService;
        private readonly AssetProvider _assetProvider;

        private readonly List<PlayerController> _players = new();
        private readonly List<EnemyBase> _enemies = new();

        public IReadOnlyList<PlayerController> Players => _players;
        public IReadOnlyList<EnemyBase> Enemies => _enemies;

        private HeartTracker _heartTracker;
        private Level _gameLevel;
        private PlayerController _mainPlayer;
        private List<GameObject> _levelHolder;
        private Finish _finish;

        public GameFactory(SaveLoadService saveLoadService,
            LeaderboardService leaderboardService, AssetProvider assetProvider,
            LevelResourceService levelResourceService)
        {
            _saveLoadService = saveLoadService;
            _leaderboardService = leaderboardService;
            _assetProvider = assetProvider;
            _levelResourceService = levelResourceService;
        }

        public ObservableVariable<int> EnemiesCounter { get; private set; }
        public ObservableVariable<int> PlayersCounter { get; private set; }

        public IEnumerator Initialize()
        {
            EnemiesCounter = new ObservableVariable<int>(Enemies.Count);
            PlayersCounter = new ObservableVariable<int>(Players.Count);
            _levelHolder = new List<GameObject>();
            yield break;
        }

        public Level CreateLevel() =>
            _gameLevel = _assetProvider.CreateLevel(_levelResourceService.Current.Value);

        public Level GetCurrentLevel() => _gameLevel;

        public Finish GetFinish() => _finish;

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
                    rb.velocity = source.GetComponent<Rigidbody>().velocity;
                }

                CopyTransformData(source, target);
            }
        }

        public Platform GetPlatform() => Object.FindObjectOfType<Platform>();

        public PlayerController GetMainPlayer() => _mainPlayer;

        public Spawner GetSpawner() => Object.FindObjectOfType<Spawner>();

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

        public void DestroyLastPlayer()
        {
            int playersCount = _players.Count - 1;
            PlayerController playerController = _players[playersCount];
            Object.Destroy(playerController.gameObject);
            RemovePlayer(playerController);
        }

        public void RemoveEnemy(EnemyBase enemy)
        {
            _enemies.Remove(enemy);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void AddEnemy(EnemyType enemyType, Vector3 position, float rotation)
        {
            EnemyBase enemy = enemyType switch
            {
                EnemyType.Simple or EnemyType.WithGun or EnemyType.WithGunAndShield =>
                    _assetProvider.CreateSimpleEnemy(position, Quaternion.Euler(0f, 180f, 0f)),
                EnemyType.Big => _assetProvider.CreateBigEnemy(position, Quaternion.Euler(0f, 180f, 0f)),
                EnemyType.Large => _assetProvider.CreateLargeEnemy(position, Quaternion.Euler(0f, 180f, 0f)),
                _ => throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null)
            };

            enemy.transform.Rotate(0, rotation, 0);
            enemy.transform.Translate(new Vector3(0, 0, -16f));
            _enemies.Add(enemy);
            _levelHolder.Add(enemy.gameObject);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void RemovePlayer(PlayerController player)
        {
            _players.Remove(player);
            player.Disable();
            PlayersCounter.Value = _players.Count;
            Object.Destroy(player.gameObject);
        }

        public void AddPlayer(PlayerController player)
        {
            _players.Add(player);
            PlayersCounter.Value = _players.Count;
        }

        public PlayerController GetNewPlayer()
        {
            float spawnGap = Random.Range(2f, 5f);
            Vector3 randomPos = Random.onUnitSphere * spawnGap;

            GameObject root = GetMainPlayer().gameObject;

            PlayerController player =
                _assetProvider.CreatePlayer(root, root.transform.position + randomPos, root.transform.rotation);

            player.Initialize();
            CopyTransformData(root.transform, player.transform);
            AddPlayer(player);
            return player;
        }

        public PlayerController CreateMainPlayer()
        {
            var startPosition = new Vector3(0, 1.75f, -1);
            _mainPlayer = _assetProvider.CreatePlayer(startPosition);
            Rigidbody capsule = Object.FindObjectOfType<Slingshot>().Capsule;
            var fixedJoint = _mainPlayer.SelfHips.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = capsule;
            _mainPlayer.SetInitial(fixedJoint, capsule.transform);
            AddPlayer(_mainPlayer);
            SetPlayerCamera();

            _levelHolder.Add(_mainPlayer.gameObject);
            return _mainPlayer;
        }

        public Finish CreateFinish(Vector3 vector3, Quaternion identity)
        {
            _finish = _assetProvider.GetFinish(vector3, identity);
            _levelHolder.Add(_finish.gameObject);
            return _finish;
        }

        public GameObject GetEnemyRagdoll(Vector3 transformPosition, Quaternion identity)
        {
            var a = _assetProvider.CreateEnemyRagdoll(transformPosition, identity);
            _levelHolder.Add(a);
            return a;
        }

        public void GetPlayerRagdoll(Vector3 transformPosition, Quaternion identity)
        {
            var a = _assetProvider.CreatePlayerRagdoll(transformPosition, identity);
            _levelHolder.Add(a);
        }

        public void GetSmoke(Vector3 vector3, Quaternion euler)
        {
            var s = _assetProvider.CreateSmoke(vector3, euler);
            _levelHolder.Add(s);
        }

        public void CreateBarrel(Vector3 randomPosition)
        {
            ExplosionBarrel barrel = _assetProvider.CreateBarrel(randomPosition);
            _levelHolder.Add(barrel.gameObject);
        }

        public RingHolder GetRing(Vector3 calculateRingPosition, Spawner.Colors[] colorArray, int index)
        {
            RingHolder ringHolder = _assetProvider.CreateRing(calculateRingPosition, colorArray, index);
            _levelHolder.Add(ringHolder.gameObject);
            return ringHolder;
        }

        public void CreateSlingshot(Vector3 position)
        {
            GameObject slingshot = _assetProvider.CreateSlingshot(position);
            _levelHolder.Add(slingshot.gameObject);
        }

        public Indicator GetIndicator() => Object.FindObjectOfType<Indicator>();

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

        public void SetPlayerCamera()
        {
            var offset = new Vector3(0, 7, -14);

            var cameraSetup = Object.FindObjectOfType<CameraSetup>();
            CameraFollow follow = cameraSetup.CameraFollow;
            follow.transform.position = _mainPlayer.SelfHips.transform.position + offset;
            follow.enabled = true;
            follow.transform.rotation = Quaternion.Euler(new Vector3(10, 0, 0));
            cameraSetup.MainCamera.fieldOfView = 60;

            follow.Setup(() =>
            {
                if (_mainPlayer != null && _mainPlayer.SelfHips != null)
                {
                    follow.enabled = true;
                    return _mainPlayer.SelfHips.transform.position + offset;
                }

                follow.enabled = false;
                return Vector3.zero;
            }, moveSpeed: 500f);
        }

        public void SetFinishCamera(float finishZPosition)
        {
            var cameraSetup = Object.FindObjectOfType<CameraSetup>();
            CameraFollow follow = cameraSetup.CameraFollow;
            cameraSetup.MainCamera.fieldOfView = 90;
            follow.SetMoveSpeed(-1);
            follow.enabled = false;
            follow.transform.position = new Vector3(0, 20, finishZPosition - 35f);
            follow.transform.rotation = Quaternion.Euler(new Vector3(50, 0, 0));
        }
    }
}
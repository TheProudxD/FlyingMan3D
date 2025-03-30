using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using BhorGames.Mechanics;
using Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class GameFactory : IService, IInitializable
    {
        private readonly LevelResourceService _levelResourceService;
        private readonly SaveLoadService _saveLoadService;
        private readonly LeaderboardService _leaderboardService;
        private readonly AssetProvider _assetProvider;

        private readonly List<PlayerController> _players = new();
        private readonly List<Enemy> _enemies = new();

        public IReadOnlyList<PlayerController> Players => _players;
        public IReadOnlyList<Enemy> Enemies => _enemies;

        private Score _score;
        private HeartTracker _heartTracker;
        private Level _gameLevel;
        private PlayerController _player;
        private List<PlayerController> _players1 = new();

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
            _score = new Score();
            EnemiesCounter = new ObservableVariable<int>(Enemies.Count);
            PlayersCounter = new ObservableVariable<int>(Players.Count);
            yield break;
        }

        public Score GetScore() => _score;

        public Level CreateLevel(int level) =>
            _gameLevel = _assetProvider.CreateLevel(level);

        public Level GetCurrentLevel() => _gameLevel;

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

        public PlayerController GetPlayer() => _player;

        public Spawner GetSpawner() => Object.FindObjectOfType<Spawner>();

        public void DestroyPlayers()
        {
            foreach (PlayerController player in Players)
            {
                Object.Destroy(player.gameObject);
            }

            _players.Clear();
        }

        public void DestroyLastPlayer()
        {
            PlayerController playerController = Players[^1];
            Object.Destroy(playerController.gameObject);
            RemovePlayer(playerController);
            // _players.RemoveAt(Players.Count - 1);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void AddEnemy(Vector3 position, float rotation)
        {
            Enemy enemy = _assetProvider.GetEnemy(position, Quaternion.Euler(0f, 180f, 0f));
            enemy.transform.Rotate(0, rotation, 0);
            enemy.transform.Translate(new Vector3(0, 0, -16f));
            _enemies.Add(enemy);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void RemovePlayer(PlayerController player)
        {
            _players.Remove(player);
            PlayersCounter.Value = _players.Count;
        }

        public void AddPlayer(PlayerController player)
        {
            _players.Add(player);
            PlayersCounter.Value = _players.Count;
        }

        public PlayerController GetNewPlayer(GameObject root)
        {
            float spawnGap = Random.Range(1.3f, 3f);
            Vector3 randomPos = Random.onUnitSphere * spawnGap;

            PlayerController player =
                _assetProvider.CreatePlayer(root, root.transform.position + randomPos, root.transform.rotation);

            player.Initialize();
            CopyTransformData(root.transform, player.transform);

            //player.transform.Rotate(player.transform.right, rot, Space.Self);
            AddPlayer(player);
            return player;
        }

        public void CreatePlayer()
        {
            var startPosition = new Vector3(0, 1.75f, -1);
            _player = _assetProvider.CreatePlayer(startPosition);
            var capsule = GameObject.Find("Capsule").GetComponent<Rigidbody>();
            var fixedJoint = _player.SelfHips.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = capsule;
            _player.SetInitial(fixedJoint, capsule.transform);
            AddPlayer(_player);
            var camera = GameObject.Find("Cinemachine").GetComponent<CinemachineVirtualCamera>();
            camera.Follow = _player.SelfHips.transform;
        }
    }
}
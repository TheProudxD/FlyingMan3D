using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using BhorGames.Mechanics;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class GameFactory : IService, IInitializable
    {
        private readonly LevelResourceService _levelResourceService;
        private readonly SaveLoadService _saveLoadService;
        private readonly LeaderboardService _leaderboardService;
        private readonly AssetProvider _assetProvider;

        public List<PlayerController> players;
        public List<EnemyFinish> Enemies = new();
        
        private Score _score;
        private HeartTracker _heartTracker;
        private Level _gameLevel;
        private PlayerController _player;
        private float spawnGap = 1.8f;

        public GameFactory(SaveLoadService saveLoadService,
            LeaderboardService leaderboardService, AssetProvider assetProvider,
            LevelResourceService levelResourceService)
        {
            _saveLoadService = saveLoadService;
            _leaderboardService = leaderboardService;
            _assetProvider = assetProvider;
            _levelResourceService = levelResourceService;
        }

        public IEnumerator Initialize()
        {
            _score = new Score();
            _player = Object.FindObjectOfType<PlayerController>();
            yield break;
        }

        public Score GetScore() => _score;

        public Level CreateLevel(int level) =>
            _gameLevel = _assetProvider.CreateLevel(level);

        public Level GetCurrentLevel() => _gameLevel;

        public PlayerController GetNewPlayer(GameObject root)
        {
            if (players == null || players.Count == 0)
            {
                players = new List<PlayerController>();
            }

            Vector3 randomPos = Random.onUnitSphere * spawnGap;

            PlayerController player = Object
                .Instantiate(root, root.transform.position + randomPos, root.transform.rotation)
                .GetComponent<PlayerController>();

            CopyTransformData(root.transform, player.transform);

            players.Add(player);
            return player;
        }

        private void CopyTransformData(Transform sourceTransform, Transform targetTransform)
        {
            if (sourceTransform.childCount != targetTransform.childCount)
            {
                UnityEngine.Debug.LogError("Players have different hierarchies!");
            }

            for (int i = 0; i < sourceTransform.childCount; i++)
            {
                var source = sourceTransform.GetChild(i);
                var target = targetTransform.GetChild(i);

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
    }
}
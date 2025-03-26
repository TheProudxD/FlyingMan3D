using System.Collections;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class GameFactory : IService, IInitializable
    {
        private readonly LevelResourceService _levelResourceService;
        private readonly SaveLoadService _saveLoadService;
        private readonly LeaderboardService _leaderboardService;
        private readonly AssetProvider _assetProvider;

        private Score _score;
        private HeartTracker _heartTracker;
        private Level _gameLevel;

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
            yield break;
        }

        public Score GetScore() => _score;

        public Level CreateLevel(int level) =>
            _gameLevel = _assetProvider.CreateLevel(level);

        public Level GetCurrentLevel() => _gameLevel;
    }
}
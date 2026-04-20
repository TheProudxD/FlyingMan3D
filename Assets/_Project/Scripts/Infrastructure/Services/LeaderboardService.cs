using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services
{
    public class LeaderboardService : IService
    {
        private const string StubPrefix = "[LeaderboardService:STUB]";
        private readonly SaveLoadService _saveLoadService;
        private readonly IPersistentProgressService _progressService;
        private readonly string _leaderboardName = "highestLevel";

        public LeaderboardService(SaveLoadService saveLoadService, IPersistentProgressService progressService)
        {
            _saveLoadService = saveLoadService;
            _progressService = progressService;
        }

        public int GetMaxLeaderboardScore() => _progressService.Progress.RichestLevel.Value;

        public void SetMaxLeaderboardScore(int current)
        {
            _progressService.Progress.RichestLevel.Value = current;
            _saveLoadService.Save();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.Log($"{StubPrefix} Saved local best score {current} for '{_leaderboardName}'. Remote submit is not integrated.");
#endif
        }
    }
}

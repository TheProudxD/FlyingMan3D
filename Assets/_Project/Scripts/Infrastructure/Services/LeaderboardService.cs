using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using YG;

namespace _Project.Scripts.Infrastructure.Services
{
    public class LeaderboardService : IService
    {
        private readonly SaveLoadService _saveLoadService;
        private readonly IPersistentProgressService _progressService;

#pragma warning disable CS0414 // Field is assigned but its value is never used
        private readonly string _leaderboardName = "highestLevel";
#pragma warning restore CS0414 // Field is assigned but its value is never used

        public LeaderboardService(SaveLoadService saveLoadService, IPersistentProgressService progressService)
        {
            _saveLoadService = saveLoadService;
            _progressService = progressService;
        }

        public int GetMaxLeaderboardScore() => _progressService.Progress.RichestLevel.Value;

        public void SetMaxLeaderboardScore(int current)
        {
            _progressService.Progress.RichestLevel.Value = current;
            // YG2.SetLeaderboard(_leaderboardName, current);
            _saveLoadService.Save();
        }
    }
}
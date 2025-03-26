using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using YG;

namespace _Project.Scripts.Infrastructure.Services
{
    public class LeaderboardService : IService
    {
        private readonly SaveLoadService _saveLoadService;
        
        private readonly string _leaderboardName = "highestLevel";

        public LeaderboardService(SaveLoadService saveLoadService) => _saveLoadService = saveLoadService;

        public int GetMaxLeaderboardScore() => YG2.saves.richestLevel;

        public void SetMaxLeaderboardScore(int current)
        {
            YG2.saves.richestLevel = current;
            YG2.SetLeaderboard(_leaderboardName, current);
            _saveLoadService.Save();
        }
    }
}
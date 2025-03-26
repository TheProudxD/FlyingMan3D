using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using YG;

namespace _Project.Scripts.Infrastructure.Services
{
    public class StatisticsService : IService
    {
        private readonly SaveLoadService _saveLoadService;

        public StatisticsService(SaveLoadService saveLoadService) => _saveLoadService = saveLoadService;

        public void IncreaseGamesPlayedNumberCounter(int value = 1)
        {
            _saveLoadService.Save();
        }

        public void IncreaseWatermelonGuessedCounter(int value = 1)
        {
            _saveLoadService.Save();
        }

        public void IncreaseFruitsMergedCounter(int value = 1)
        {
            _saveLoadService.Save();
        }
    }
}
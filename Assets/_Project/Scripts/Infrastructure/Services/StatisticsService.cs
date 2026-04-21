using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services
{
    public class StatisticsService : IService
    {
        private const string StubPrefix = "[StatisticsService:STUB]";

        private readonly SaveLoadService _saveLoadService;

        public StatisticsService(SaveLoadService saveLoadService) => _saveLoadService = saveLoadService;

        public void IncreaseGamesPlayedNumberCounter(int value = 1)
        {
            LogStub("gamesPlayed", value);
            _saveLoadService.Save();
        }

        public void IncreaseWatermelonGuessedCounter(int value = 1)
        {
            LogStub("watermelonGuessed", value);
            _saveLoadService.Save();
        }

        public void IncreaseFruitsMergedCounter(int value = 1)
        {
            LogStub("fruitsMerged", value);
            _saveLoadService.Save();
        }

        private void LogStub(string counterName, int value)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.Log($"{StubPrefix} '{counterName}' += {value}. Persistent statistics are not integrated.");
#endif
        }
    }
}

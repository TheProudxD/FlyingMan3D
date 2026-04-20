using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services
{
    public class MetricService : IService
    {
        private const string StubPrefix = "[MetricService:STUB]";

        public void GameLoaded() => Track("gameLoaded");

        public void LevelStarted(int level) => Track("levelStarted", "level", level.ToString());

        public void LevelPassed(int level) => Track("levelPassed", "level", level.ToString());

        public void LevelLost(int level) => Track("levelLost", "level", level.ToString());

        public void GameContinuedForAd() => Track("gameContinuedForAd");

        public void StatisticsViewed() => Track("statisticsViewed");

        public void ReviewSent() => Track("sentReview");

        public void OpenedMoreGames() => Track("openedMoreGames");

        public void TutorialPassed() => Track("tutorialPassed");

        public void LevelSkippedForAd() => Track("levelSkippedForAd");

        private void Track(string eventName, string key = null, string value = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (string.IsNullOrEmpty(key))
            {
                Debug.Log($"{StubPrefix} {eventName}");
                return;
            }

            Debug.Log($"{StubPrefix} {eventName} ({key}: {value})");
#endif
        }
    }
}

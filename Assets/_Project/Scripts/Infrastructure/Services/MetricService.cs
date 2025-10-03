using YG;

namespace _Project.Scripts.Infrastructure.Services
{
    public class MetricService : IService
    {
        public void GameLoaded()
        {
            //YG2.MetricaSend("gameLoaded");
        }

        public void LevelStarted(int level)
        {
            //YG2.MetricaSend("levelStarted", "level", level.ToString());
        }

        public void LevelPassed(int level)
        {
            //YG2.MetricaSend("levelPassed", "level", level.ToString());
        }

        public void LevelLost(int level)
        {
            //YG2.MetricaSend("levelLost", "level", level.ToString());
        }

        public void GameContinuedForAd()
        {
            //YG2.MetricaSend("gameContinuedForAd");
        }

        public void StatisticsViewed()
        {
            //YG2.MetricaSend("statisticsViewed");
        }

        public void ReviewSent()
        {
            //YG2.MetricaSend("sentReview");
        }

        public void OpenedMoreGames()
        {
            //YG2.MetricaSend("openedMoreGames");
        }

        public void TutorialPassed()
        {
            //YG2.MetricaSend("tutorialPassed");
        }

        public void LevelSkippedForAd()
        {
            //YG2.MetricaSend("levelSkippedForAd");
        }
    }
}
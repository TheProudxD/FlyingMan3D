using YG;

namespace _Project.Scripts.Infrastructure.Services
{
    public class MetricService : IService
    {
        public void GameStarted() => YG2.MetricaSend("gameStarted");

        public void LevelPassed() => YG2.MetricaSend("gameFinished");

        public void GameContinuedForAd() => YG2.MetricaSend("gameContinuedForAd");

        public void StatisticsViewed() => YG2.MetricaSend("statisticsViewed");

        public void ReviewSent() => YG2.MetricaSend("sentReview");

        public void OpenedMoreGames() => YG2.MetricaSend("openedMoreGames");

        public void TutorialPassed() => YG2.MetricaSend("tutorialPassed");

        public void LevelLost()
        {
            
        }

        public void HintUsed()
        {
        }
    }
}
using _Project.Scripts.Infrastructure.Services.LevelSystem;

namespace _Project.Scripts.UI.Views
{
    public class ScoreView : ScoreBaseView
    {
        private Score _score;

        public override void Initialize()
        {
            _score = GameFactory.GetScore();
            _score.Value.ChangedWithOld += OnScoreChanged;

            DisplayDefaultScore();
        }

        private void OnDestroy()
        {
            _score.Value.ChangedWithOld -= OnScoreChanged;
        }

        protected override void DisplayDefaultScore()
        {
            if (_score != null)
            {
                OnScoreChanged(_score.Value.Value, _score.Value.Value);
            }
            else
            {
                OnScoreChanged(Score.DEFAULT_VALUE, Score.DEFAULT_VALUE);
            }
        }

        private void OnScoreChanged(int oldScore, int score) =>
            AnimationService.ResourceChanged(transform, oldScore, score, IncrementDuration,
                x => Text.SetText(x.ToString(@"mm\:ss")));
    }
}
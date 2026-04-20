using _Project.Scripts.Infrastructure.Services.LevelSystem;

namespace _Project.Scripts.UI.Views
{
    public class ScoreView : ScoreBaseView
    {
        private Score _score;

        public override void Initialize()
        {
            DisplayDefaultScore();
        }

        public void Bind(Score score)
        {
            if (ReferenceEquals(_score, score))
            {
                DisplayDefaultScore();
                return;
            }

            Unsubscribe();
            _score = score;
            Subscribe();
            DisplayDefaultScore();
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

        private void OnDestroy() => Unsubscribe();

        private void Subscribe()
        {
            if (_score == null)
                return;

            _score.Value.ChangedWithOld += OnScoreChanged;
        }

        private void Unsubscribe()
        {
            if (_score == null)
                return;

            _score.Value.ChangedWithOld -= OnScoreChanged;
        }
    }
}

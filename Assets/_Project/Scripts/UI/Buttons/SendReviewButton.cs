using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Review;
using _Project.Scripts.Tools.Extensions;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Buttons
{
    public class SendReviewButton : ButtonBase
    {
        [Inject] private ReviewShowService _reviewShowService;
        [Inject] private MetricService _metricService;

        protected override void OnClick()
        {
            gameObject.Deactivate();
            _reviewShowService.Show(OnReviewSent);
        }

        private void OnReviewSent(bool success)
        {
            if (!success)
                return;

            //_windowService.Show(WindowId.ReviewThanks);
            _metricService.ReviewSent();
        }
    }
}
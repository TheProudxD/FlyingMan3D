using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Review;
using _Project.Scripts.Tools.Extensions;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Buttons
{
    public class SendReviewButton : ButtonBase
    {
        private const string StubPrefix = "[SendReviewButton:STUB]";

        [Inject] private ReviewShowService _reviewShowService;
        [Inject] private MetricService _metricService;

        protected override void OnClick()
        {
            if (!_reviewShowService.CanShow())
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log($"{StubPrefix} Review flow is unavailable, click ignored.");
#endif
                return;
            }

            gameObject.Deactivate();
            _reviewShowService.Show(OnReviewSent);
        }

        private void OnReviewSent(bool success)
        {
            if (!success)
            {
                gameObject.Activate();
                return;
            }

            //_windowService.Show(WindowId.ReviewThanks);
            _metricService.ReviewSent();
        }
    }
}

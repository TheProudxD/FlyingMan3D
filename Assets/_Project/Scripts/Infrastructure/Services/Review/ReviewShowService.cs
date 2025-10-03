using System;
using YG;

namespace _Project.Scripts.Infrastructure.Services.Review
{
    public class ReviewShowService : IService
    {
        private Action<bool> _onReviewSentAction;
        
        // public ReviewShowService() => YG2.onReviewSent += OnReviewSent;

        // ~ReviewShowService() => YG2.onReviewSent -= OnReviewSent;
        
        private void OnReviewSent(bool obj) => _onReviewSentAction?.Invoke(obj);

        public void Show(Action<bool> onReviewSent = null)
        {
            _onReviewSentAction = onReviewSent;
            // YG2.ReviewShow();
        }

        public bool CanShow() => false;
    }
}
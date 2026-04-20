using System;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Review
{
    public class ReviewShowService : IService
    {
        private const string StubPrefix = "[ReviewShowService:STUB]";
        private Action<bool> _onReviewSentAction;

        private void OnReviewSent(bool success) => Complete(success);

        public void Show(Action<bool> onReviewSent = null)
        {
            _onReviewSentAction = onReviewSent;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.Log($"{StubPrefix} Review flow is not integrated yet.");
#endif

            Complete(false);
        }

        public bool CanShow() => false;

        private void Complete(bool success)
        {
            Action<bool> callback = _onReviewSentAction;
            _onReviewSentAction = null;
            callback?.Invoke(success);
        }
    }
}

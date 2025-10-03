using System;
using YG;

namespace _Project.Scripts.Infrastructure.Services
{
    public class AdsService
    {
        public void PlayInterstitial()
        {
            // if (YG2.isTimerAdvCompleted)
                // YG2.InterstitialAdvShow();
        }

        public void PlayRewardedVideo(string id, Action onRewarded)
        {
            // YG2.RewardedAdvShow(id, onRewarded);
        }
    }
}
using System;

namespace _Project.Scripts.Infrastructure.Services
{
    public class AdsService : IInitializable
    {
        private readonly AppOpenAdController _appOpenAdController = new();
        private readonly RewardedAdController _rewardedAdController = new();
        private readonly InterstitialAdController _interstitialAdController = new();

        public void Initialize()
        {
            _appOpenAdController.Initialize();
            _interstitialAdController.Initialize();
            _rewardedAdController.Initialize();
        }

        public void PlayInterstitial()
        {
            _interstitialAdController.Show();
        }

        public void PlayRewardedVideo(string id, Action onRewarded)
        {
            _rewardedAdController.Show(id, onRewarded);
        }
    }
}
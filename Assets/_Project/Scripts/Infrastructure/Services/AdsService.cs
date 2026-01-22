using System;

namespace _Project.Scripts.Infrastructure.Services
{
    public class AdsService : IInitializable, IDisposable
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

        public void Dispose()
        {
            _appOpenAdController.Dispose();
            _rewardedAdController.Dispose();
            _interstitialAdController.Dispose();
        }

        public void PlayInterstitial()
        {
            _interstitialAdController.Show();
        }

        public void PlayRewardedVideo(string id, Action onRewarded)
        {
            _rewardedAdController.Show(onRewarded);
        }
    }
}
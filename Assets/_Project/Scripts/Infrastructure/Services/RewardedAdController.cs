using System;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace _Project.Scripts.Infrastructure.Services
{
    public sealed class RewardedAdController
    {
        private string _message = "";
        private RewardedAdLoader _rewardedAdLoader;
        private RewardedAd _rewardedAd;

        public void Initialize()
        {
            SetupLoader();
            RequestRewardedAd();
        }

        private void SetupLoader()
        {
            _rewardedAdLoader = new RewardedAdLoader();
            _rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            _rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        }

        private void RequestRewardedAd()
        {
            DisplayMessage("RewardedAd is not ready yet");
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            _rewardedAd?.Destroy();

            string adUnitId = "R-M-17516562-3";

            _rewardedAdLoader.LoadAd(CreateAdRequest(adUnitId));
            DisplayMessage("Rewarded Ad is requested");
        }

        public void Show(Action onRewarded)
        {
            if (_rewardedAd == null)
            {
                DisplayMessage("RewardedAd is not ready yet");
                return;
            }

            _rewardedAd.OnRewarded += RewardedAdOnOnRewarded(onRewarded);
            _rewardedAd.Show();
        }

        private EventHandler<Reward> RewardedAdOnOnRewarded(Action onRewarded) =>
            (x, y) =>
            {
                if (_rewardedAd != null)
                    _rewardedAd.OnRewarded -= RewardedAdOnOnRewarded(onRewarded);

                onRewarded?.Invoke();
            };

        private AdRequestConfiguration CreateAdRequest(string adUnitId) =>
            new AdRequestConfiguration.Builder(adUnitId).Build();

        private void DestroyAd()
        {
            _rewardedAd?.Destroy();
            _rewardedAd = null;
        }

        private void DisplayMessage(string message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            UnityEngine.Debug.Log(message);
        }

        #region Rewarded Ad callback handlers

        private void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");
            _rewardedAd = args.RewardedAd;
            _rewardedAd.OnAdClicked += HandleAdClicked;
            _rewardedAd.OnAdShown += HandleAdShown;
            _rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
            _rewardedAd.OnAdImpression += HandleImpression;
            _rewardedAd.OnAdDismissed += HandleAdDismissed;
            _rewardedAd.OnRewarded += HandleRewarded;
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            DisplayMessage(
                $"HandleAdFailedToLoad event received with message: {args.Message}");

            DestroyAd();
            RequestRewardedAd();
        }

        private void HandleAdClicked(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdClicked event received");
        }

        private void HandleAdShown(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdShown event received");
        }

        private void HandleAdDismissed(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdDismissed event received");
            DestroyAd();
            RequestRewardedAd();
        }

        private void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage($"HandleImpression event received with data: {data}");
        }

        private void HandleRewarded(object sender, Reward args)
        {
            DisplayMessage($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DisplayMessage(
                $"HandleAdFailedToShow event received with message: {args.Message}");
        }

        #endregion
    }
}
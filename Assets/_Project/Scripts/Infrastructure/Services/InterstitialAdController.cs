using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace _Project.Scripts.Infrastructure.Services
{
    public sealed class InterstitialAdController : IDisposable
    {
        private string _message = "";
        private InterstitialAdLoader _interstitialAdLoader;
        private Interstitial _interstitial;

        public void Initialize()
        {
            SetupLoader();
            RequestInterstitial();
        }

        public void Dispose()
        {
            UnsubscribeEvents();
            UnsubscribeLoaderEvents();
            DestroyAd();
            _interstitialAdLoader = null;
        }

        private void SetupLoader()
        {
            _interstitialAdLoader = new InterstitialAdLoader();
            SubscribeLoaderEvents();
        }

        private void SubscribeLoaderEvents()
        {
            _interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
            _interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        }

        private void UnsubscribeLoaderEvents()
        {
            if (_interstitialAdLoader != null)
            {
                _interstitialAdLoader.OnAdLoaded -= HandleAdLoaded;
                _interstitialAdLoader.OnAdFailedToLoad -= HandleAdFailedToLoad;
            }
        }

        private void SubscribeEvents()
        {
            if (_interstitial != null)
            {
                _interstitial.OnAdClicked += HandleAdClicked;
                _interstitial.OnAdShown += HandleAdShown;
                _interstitial.OnAdFailedToShow += HandleAdFailedToShow;
                _interstitial.OnAdImpression += HandleImpression;
                _interstitial.OnAdDismissed += HandleAdDismissed;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_interstitial != null)
            {
                _interstitial.OnAdClicked -= HandleAdClicked;
                _interstitial.OnAdShown -= HandleAdShown;
                _interstitial.OnAdFailedToShow -= HandleAdFailedToShow;
                _interstitial.OnAdImpression -= HandleImpression;
                _interstitial.OnAdDismissed -= HandleAdDismissed;
            }
        }

        private void RequestInterstitial()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            string adUnitId = "R-M-17516562-1";

            if (_interstitial != null)
            {
                _interstitial.Destroy();
            }

            _interstitialAdLoader.LoadAd(CreateAdRequest(adUnitId));
            DisplayMessage("Interstitial is requested");
        }

        public void Show()
        {
            if (_interstitial == null)
            {
                DisplayMessage("Interstitial is not ready yet");
                return;
            }

            SubscribeEvents();
            _interstitial.Show();
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId) =>
            new AdRequestConfiguration.Builder(adUnitId).Build();

        private void DisplayMessage(string message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            UnityEngine.Debug.Log(message);
        }

        #region Interstitial callback handlers

        private void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");

            // Unsubscribe from previous ad events if any
            UnsubscribeEvents();

            _interstitial = args.Interstitial;
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            DisplayMessage($"HandleAdFailedToLoad event received with message: {args.Message}");
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

            // Clean up subscriptions and destroy ad
            UnsubscribeEvents();
            DestroyAd();
        }

        private void DestroyAd()
        {
            if (_interstitial != null)
            {
                _interstitial.Destroy();
                _interstitial = null;
            }
        }

        private void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage($"HandleImpression event received with data: {data}");
        }

        private void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DisplayMessage($"HandleAdFailedToShow event received with message: {args.Message}");
        }

        #endregion
    }
}
using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace _Project.Scripts.Infrastructure.Services
{
    public class AppOpenAdController : IDisposable
    {
        private string _message = "";
        private AppOpenAdLoader _appOpenAdLoader;
        private AppOpenAd _appOpenAd;
        private bool _isAdShownOcColdStart;

        public void Initialize()
        {
            SetupLoader();
            RequestAd();
            AppStateObserver.OnAppStateChanged += HandleAppStateChanged;
        }

        public void Dispose()
        {
            UnsubscribeEvents();
            UnsubscribeLoaderEvents();
            AppStateObserver.OnAppStateChanged -= HandleAppStateChanged;
            DestroyAd();
            _appOpenAdLoader = null;
        }

        private void SetupLoader()
        {
            _appOpenAdLoader = new AppOpenAdLoader();
            SubscribeLoaderEvents();
        }

        private void SubscribeLoaderEvents()
        {
            _appOpenAdLoader.OnAdLoaded += HandleAdLoaded;
            _appOpenAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        }

        private void UnsubscribeLoaderEvents()
        {
            if (_appOpenAdLoader != null)
            {
                _appOpenAdLoader.OnAdLoaded -= HandleAdLoaded;
                _appOpenAdLoader.OnAdFailedToLoad -= HandleAdFailedToLoad;
            }
        }

        private void SubscribeEvents()
        {
            if (_appOpenAd != null)
            {
                _appOpenAd.OnAdClicked += HandleAdClicked;
                _appOpenAd.OnAdShown += HandleAdShown;
                _appOpenAd.OnAdFailedToShow += HandleAdFailedToShow;
                _appOpenAd.OnAdImpression += HandleImpression;
                _appOpenAd.OnAdDismissed += HandleAdDismissed;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_appOpenAd != null)
            {
                _appOpenAd.OnAdClicked -= HandleAdClicked;
                _appOpenAd.OnAdShown -= HandleAdShown;
                _appOpenAd.OnAdFailedToShow -= HandleAdFailedToShow;
                _appOpenAd.OnAdImpression -= HandleImpression;
                _appOpenAd.OnAdDismissed -= HandleAdDismissed;
            }
        }

        private void RequestAd()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            string adUnitId = "R-M-17516562-2";

            if (_appOpenAd != null)
            {
                _appOpenAd.Destroy();
            }

            _appOpenAdLoader.LoadAd(CreateAdRequestConfiguration(adUnitId));
            DisplayMessage("AppOpenAd is requested");
        }

        private void Show()
        {
            if (_appOpenAd == null)
            {
                DisplayMessage("AppOpenAd is not ready yet");
                return;
            }

            _appOpenAd.Show();
        }

        private AdRequestConfiguration CreateAdRequestConfiguration(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        private void DisplayMessage(string message)
        {
            _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
            UnityEngine.Debug.Log(message);
        }

        #region AppOpenAd callback handlers

        public void HandleAppStateChanged(object sender, AppStateChangedEventArgs args)
        {
            if (_appOpenAd != null && args.IsInBackground == false)
            {
                Show();
            }
        }

        public void HandleAdLoaded(object sender, AppOpenAdLoadedEventArgs args)
        {
            DisplayMessage("HandleAdLoaded event received");

            // Unsubscribe from previous ad events if any
            UnsubscribeEvents();

            _appOpenAd = args.AppOpenAd;
            SubscribeEvents();

            if (!_isAdShownOcColdStart)
            {
                Show();
                _isAdShownOcColdStart = true;
            }
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            DisplayMessage("HandleAdFailedToLoad event received with message: " + args.Message);
            DestroyAd();
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdClicked event received");
        }

        public void HandleAdShown(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdShown event received");
        }

        public void HandleAdDismissed(object sender, EventArgs args)
        {
            DisplayMessage("HandleAdDismissed event received");

            // Clean up subscriptions and request new ad
            UnsubscribeEvents();
            DestroyAd();
            RequestAd();
        }

        private void DestroyAd()
        {
            _appOpenAd?.Destroy();
            _appOpenAd = null;
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            DisplayMessage($"HandleImpression event received with data: {data}");
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            DisplayMessage($"HandleAdFailedToShow event received with message: {args.Message}");
        }

        #endregion
    }
}
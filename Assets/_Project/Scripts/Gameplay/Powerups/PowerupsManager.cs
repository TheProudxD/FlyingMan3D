using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools.Extensions;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YG;

namespace UI
{
    public class PowerupsManager : MonoBehaviour
    {
        [Inject] private AudioService _audioService;
        [Inject] private AdsService _adsService;
        [Inject] private MoneyResourceService _moneyResourceService;

        [SerializeField]
        private PowerupView _healthView;

        [SerializeField]
        private PowerupView _speedView;

        [FormerlySerializedAs("_launchForceView")] [SerializeField]
        private PowerupView _flyingControlView;

        private List<Button.ButtonClickedEvent> _buttonAnimations;
        private readonly List<PowerupView> _powerupData = new();

        private void Awake()
        {
            _powerupData.Add(_healthView);
            _powerupData.Add(_speedView);
            _powerupData.Add(_flyingControlView);
        }

        private void OnEnable()
        {
            GetPowerupData();
        }
        
        private void GetPowerupData()
        {
            foreach (PowerupView powerupData in _powerupData)
            {
                powerupData.Initialize();
                powerupData.PriceText.text = powerupData.Price.ToString();
                powerupData.LevelText.text = powerupData.Progress.ToString();
            }

            CheckForPriceOrAdPurchase();
        }

        private void BuyPowerup(PowerupView powerupView)
        {
            if (_moneyResourceService.ObservableValue.Value < powerupView.Price)
                return;

            _moneyResourceService.Spend(this, powerupView.Price);

            powerupView.IncreaseValue();

            CheckForPriceOrAdPurchase();
        }

        public void CheckForPriceOrAdPurchase()
        {
            int currentMoneyAmount = _moneyResourceService.ObservableValue.Value;

            foreach (PowerupView powerup in _powerupData)
            {
                powerup.BuyButton.onClick.RemoveAllListeners();

                if (currentMoneyAmount < powerup.Price)
                {
                    powerup.AdIcon.SetActive(true);
                    powerup.PriceIcon.SetActive(false);
                    ToggleButtonToAd(powerup);
                }
                else
                {
                    powerup.PriceIcon.SetActive(true);
                    powerup.AdIcon.SetActive(false);
                    ToggleButtonToBuy(powerup);
                }
            }
        }

        private void ToggleButtonToAd(PowerupView powerup)
        {
            powerup.BuyButton.onClick.RemoveAllListeners();

            powerup.BuyButton.Add(() =>
                {
                    _audioService.PlayClickSound();
                    _adsService.PlayRewardedVideo(powerup.Id.ToString(), () => GivePowerup(powerup.Id));
                }
            );
        }

        private void ToggleButtonToBuy(PowerupView powerup)
        {
            powerup.BuyButton.onClick.RemoveAllListeners();

            powerup.BuyButton.Add(() =>
            {
                _audioService.PlayMoneySound();
                BuyPowerup(powerup);
            });
        }

        private void GivePowerup(PowerupType id)
        {
            if ((int)id > 2) return;

            // Metrica.Instance.WatchedAdForPowerup(id.ToString());
            _powerupData.Find(p => p.Id == id).IncreaseValue();
        }
    }
}
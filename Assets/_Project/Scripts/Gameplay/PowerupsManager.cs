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
        private readonly Dictionary<PowerupType, PowerupView> _powerupData = new();

        private void Awake()
        {
            _powerupData.Add(PowerupType.Health, _healthView);
            _powerupData.Add(PowerupType.MovingSpeed, _speedView);
            _powerupData.Add(PowerupType.FlyingControl, _flyingControlView);
        }

        private void OnEnable()
        {
            YG2.onGetSDKData += GetPowerupData;
            if (YG2.isSDKEnabled)
                GetPowerupData();
        }

        private void OnDisable() => YG2.onGetSDKData -= GetPowerupData;

        private void GetPowerupData()
        {
            _healthView.Progress = YG2.saves.health;
            _speedView.Progress = YG2.saves.movingSpeedProgress;
            _flyingControlView.Progress = YG2.saves.flyingControlProgress;

            _healthView.Price = _healthView.GetPriceAtStart();
            _speedView.Price = _speedView.GetPriceAtStart();
            _flyingControlView.Price = _flyingControlView.GetPriceAtStart();

            foreach (PowerupView powerupData in _powerupData.Values)
            {
                powerupData.PriceText.text = powerupData.Price.ToString();
                powerupData.LevelText.text = powerupData.Progress.ToString();
                // powerupData.NameText.text = powerupData.Name;
            }

            CheckForPriceOrAdPurchase();
        }

        private void BuyPowerup(PowerupView powerupView)
        {
            if (_moneyResourceService.ObservableValue.Value < powerupView.Price)
                return;

            _moneyResourceService.Spend(this, powerupView.Price);

            powerupView.Price += powerupView.GetPrice();
            IncreaseValue(powerupView);

            CheckForPriceOrAdPurchase();
        }

        public void CheckForPriceOrAdPurchase()
        {
            int currentMoneyAmount = _moneyResourceService.ObservableValue.Value;

            foreach (PowerupView powerup in _powerupData.Values)
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
                _audioService.PlayClickSound();
                BuyPowerup(powerup);
            });
        }

        private void IncreaseValue(PowerupView powerupView)
        {
            powerupView.Progress++;

            switch (powerupView.Id)
            {
                case PowerupType.Health:
                    YG2.saves.health = powerupView.Progress;
                    break;
                case PowerupType.MovingSpeed:
                    YG2.saves.movingSpeed = powerupView.Progress;
                    break;
                case PowerupType.FlyingControl:
                    YG2.saves.flyingControl = powerupView.Progress;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            YG2.SaveProgress();
        }

        private void GivePowerup(PowerupType id)
        {
            if ((int)id > 2) return;

            // Metrica.Instance.WatchedAdForPowerup(id.ToString());
            IncreaseValue(_powerupData[id]);
        }
    }
}
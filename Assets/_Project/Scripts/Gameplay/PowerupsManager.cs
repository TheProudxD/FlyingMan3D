using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools.Extensions;
using Reflex.Attributes;
using UnityEngine;
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

        [SerializeField]
        private PowerupView _launchForceView;

        [SerializeField] private AnimationCurve _priceCurve;

        private List<Button.ButtonClickedEvent> _buttonAnimations;
        private readonly Dictionary<PowerupType, PowerupView> _powerupData = new();

        private void Awake()
        {
            _powerupData.Add(PowerupType.Health, _healthView);
            _powerupData.Add(PowerupType.MovingSpeed, _speedView);
            _powerupData.Add(PowerupType.LaunchForce, _launchForceView);
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
            _healthView.Level = YG2.saves.health;
            _speedView.Level = YG2.saves.movingSpeed;
            _launchForceView.Level = YG2.saves.launchForce;

            _healthView.Price = GetPriceAtStart(_healthView.Level);
            _speedView.Price = GetPriceAtStart(_speedView.Level);
            _launchForceView.Price = GetPriceAtStart(_launchForceView.Level);

            foreach (var powerupData in _powerupData.Values)
            {
                powerupData.PriceText.text = powerupData.Price.ToString();
                powerupData.LevelText.text = powerupData.Level.ToString();
                // powerupData.NameText.text = powerupData.Name;
            }

            CheckForPriceOrAdPurchase();
        }

        private void BuyPowerup(PowerupView powerupView)
        {
            if (_moneyResourceService.ObservableValue.Value < powerupView.Price)
                return;

            _moneyResourceService.Spend(this, powerupView.Price);

            powerupView.Price += GetPrice(powerupView);
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
            powerupView.Level++;

            switch (powerupView.Id)
            {
                case PowerupType.Health:
                    YG2.saves.health = powerupView.Level;
                    break;
                case PowerupType.MovingSpeed:
                    YG2.saves.movingSpeed = powerupView.Level;
                    break;
                case PowerupType.LaunchForce:
                    YG2.saves.launchForce = powerupView.Level;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            YG2.SaveProgress();
        }

        private int GetPrice(PowerupView powerupView) => (int)_priceCurve.Evaluate(powerupView.Level);

        private void GivePowerup(PowerupType id)
        {
            if ((int)id > 2) return;

            // Metrica.Instance.WatchedAdForPowerup(id.ToString());
            IncreaseValue(_powerupData[id]);
        }

        private int GetPriceAtStart(int level)
        {
            int price = 0;

            for (var i = 0; i < level; i++)
                price += (int)_priceCurve.Evaluate(i);

            return price;
        }
    }
}
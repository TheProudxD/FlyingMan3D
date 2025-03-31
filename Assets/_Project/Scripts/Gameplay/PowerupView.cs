using System;
using _Project.Scripts.Infrastructure.Services.Localization;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace UI
{
    internal class PowerupView : MonoBehaviour
    {
        [Inject] private LocalizationService _localizationService;

        [field: SerializeField] public string LocalizationName;
        [field: SerializeField] public PowerupType Id { get; private set; }

        public TextMeshProUGUI NameText;
        public TextMeshProUGUI LevelText;
        public TextMeshProUGUI PriceText;

        public Button BuyButton;

        public GameObject PriceIcon;
        public GameObject AdIcon;
        private int _price;

        public int Level
        {
            get
            {
                return Id switch
                {
                    PowerupType.Health => YG2.saves.healthProgress,
                    PowerupType.MovingSpeed => YG2.saves.movingSpeedProgress,
                    PowerupType.LaunchForce => YG2.saves.launchForceProgress,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                switch (Id)
                {
                    case PowerupType.Health:
                        YG2.saves.healthProgress = value;
                        break;
                    case PowerupType.MovingSpeed:
                        YG2.saves.movingSpeedProgress = value;
                        break;
                    case PowerupType.LaunchForce:
                        YG2.saves.launchForceProgress = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                YG2.SaveProgress();
                LevelText.SetText(value.ToString());
            }
        }

        public int Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _price = value;
                PriceText.SetText(value.ToString());
            }
        }

        private void OnEnable()
        {
            _localizationService.LocaleChanged += ChangeName;
            _localizationService.Localize(LocalizationName);
        }

        private void OnDisable()
        {
            _localizationService.LocaleChanged -= ChangeName;
        }

        private void ChangeName(LocaleConfig arg1, int arg2)
        {
            _localizationService.Localize(LocalizationName);
        }
    }

    internal enum PowerupType
    {
        Health,
        MovingSpeed,
        LaunchForce
    }
}
using System;
using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Infrastructure.Services.Localization.UI;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace UI
{
    internal class PowerupView : MonoBehaviour
    {
        [field: SerializeField] private LocalizedLabel _localizedLabel;
        [field: SerializeField] public string LocalizationName;
        [field: SerializeField] public PowerupType Id { get; private set; }
        [SerializeField] private AnimationCurve _priceCurve;

        public TextMeshProUGUI NameText;
        public TextMeshProUGUI LevelText;
        public TextMeshProUGUI PriceText;

        public Button BuyButton;

        public GameObject PriceIcon;
        public GameObject AdIcon;
        private int _price;

        public int Progress
        {
            get
            {
                return Id switch
                {
                    PowerupType.Health => YG2.saves.healthProgress,
                    PowerupType.MovingSpeed => YG2.saves.movingSpeedProgress,
                    PowerupType.FlyingControl => YG2.saves.flyingControlProgress,
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
                    case PowerupType.FlyingControl:
                        YG2.saves.flyingControlProgress = value;
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

        private void Start() => _localizedLabel.ChangeKey(LocalizationName);

        public int GetPrice() => (int)_priceCurve.Evaluate(Progress);
        
        public int GetPriceAtStart()
        {
            int price = 0;

            for (var i = 0; i < Progress; i++)
                price += (int)_priceCurve.Evaluate(i);

            return price;
        }
    }

    internal enum PowerupType
    {
        Health,
        MovingSpeed,
        FlyingControl
    }
}
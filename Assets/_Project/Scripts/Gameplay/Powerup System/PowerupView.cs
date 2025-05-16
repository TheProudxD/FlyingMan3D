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
        [field: SerializeField] public PowerupType Id { get; private set; }
        [SerializeField] private AnimationCurve _priceCurve;

        public TextMeshProUGUI NameText;
        public TextMeshProUGUI LevelText;
        public TextMeshProUGUI PriceText;

        public Button BuyButton;

        public GameObject PriceIcon;
        public GameObject AdIcon;
        private int _price;
        private int _maxProgress;

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
            private set
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
            private set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _price = value;
                PriceText.SetText(value.ToString());
            }
        }
        
        public int GetPrice() => (int)_priceCurve.Evaluate(Progress);

        public int GetPriceAtStart()
        {
            int price = 0;

            for (var i = 0; i < Progress; i++)
                price += (int)_priceCurve.Evaluate(i);

            return price;
        }

        public void IncreaseValue()
        {
            Progress++;
            Price = GetPrice();

            switch (Id)
            {
                case PowerupType.Health:
                    YG2.saves.health += YG2.saves.healthDelta;
                    break;
                case PowerupType.MovingSpeed:
                    YG2.saves.movingSpeed += YG2.saves.movingSpeedDelta;
                    break;
                case PowerupType.FlyingControl:
                    YG2.saves.flyingControl += YG2.saves.flyingControlDelta;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            YG2.SaveProgress();

            CheckCanInteract();
        }

        private void CheckCanInteract()
        {
            if (Progress >= _maxProgress)
            {
                BuyButton.interactable = false;
            }
        }

        public void Initialize()
        {
            Progress = Id switch
            {
                PowerupType.Health => YG2.saves.health,
                PowerupType.MovingSpeed => YG2.saves.movingSpeedProgress,
                PowerupType.FlyingControl => YG2.saves.flyingControlProgress,
                _ => throw new ArgumentOutOfRangeException()
            };

            _maxProgress = (int)_priceCurve.keys[_priceCurve.length - 1].time;
            Price = GetPrice();
            CheckCanInteract();
        }
    }

    internal enum PowerupType
    {
        Health,
        MovingSpeed,
        FlyingControl
    }
}
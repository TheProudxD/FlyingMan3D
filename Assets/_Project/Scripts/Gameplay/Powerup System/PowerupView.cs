using System;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace UI
{
    internal class PowerupView : MonoBehaviour
    {
        [Inject] private IPersistentProgressService _progressService;
        [Inject] private SaveLoadService _saveLoadService;
        
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
                    PowerupType.Health => _progressService.PowerupProgress.healthProgress,
                    PowerupType.MovingSpeed => _progressService.PowerupProgress.movingSpeedProgress,
                    PowerupType.FlyingControl => _progressService.PowerupProgress.flyingControlProgress,
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
                        _progressService.PowerupProgress.healthProgress = value;
                        break;
                    case PowerupType.MovingSpeed:
                        _progressService.PowerupProgress.movingSpeedProgress = value;
                        break;
                    case PowerupType.FlyingControl:
                        _progressService.PowerupProgress.flyingControlProgress = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _saveLoadService.Save();
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
                    _progressService.PowerupProgress.health += SavesStatic.healthDelta;
                    break;
                case PowerupType.MovingSpeed:
                    _progressService.PowerupProgress.movingSpeed += SavesStatic.movingSpeedDelta;
                    break;
                case PowerupType.FlyingControl:
                    _progressService.PowerupProgress.flyingControl += SavesStatic.flyingControlDelta;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _saveLoadService.Save();

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
                PowerupType.Health => _progressService.PowerupProgress.health,
                PowerupType.MovingSpeed => _progressService.PowerupProgress.movingSpeedProgress,
                PowerupType.FlyingControl => _progressService.PowerupProgress.flyingControlProgress,
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
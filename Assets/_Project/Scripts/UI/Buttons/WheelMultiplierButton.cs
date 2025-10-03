using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Localization;
using Reflex.Attributes;
using TMPro;
using TS.LocalizationSystem;
using UnityEngine;

namespace _Project.Scripts.UI.Buttons
{
    public class WheelMultiplierButton : ButtonBase
    {
        [Inject] private LocalizationService _localizationService;
        
        private readonly int[] _modifiers = { 2, 4, 6, 4, 2 };

        [SerializeField] private TargetArrowAnimation _arrowAnimation;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private TextMeshProUGUI[] _modifierTexts;

        private int _reward;
        public int Multiplier { get; private set; }

        public void SetInitReward(int reward)
        {
            _reward = reward;
            SetAmount(reward);
            _arrowAnimation.Enable();

            for (int i = 0; i < _modifiers.Length; i++)
            {
                _modifierTexts[i].SetText($"{_modifiers[i]}X");
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _arrowAnimation.Angle.Changed += RandomizeAmountOfMoney;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _arrowAnimation.Angle.Changed -= RandomizeAmountOfMoney;
        }

        private void RandomizeAmountOfMoney(float angle)
        {
            Multiplier = Mathf.Abs(angle) switch
            {
                <= 18 and >= 0 => _modifiers[2],
                <= 54 and > 18 => _modifiers[1],
                <= 90 and > 54 => _modifiers[0],
                _ => 1
            };

            int money = _reward * Multiplier;

            SetAmount(money);
        }

        protected override void OnClick()
        {
            _arrowAnimation.Disable();
        }

        private async void SetAmount(int amount)
        {
            string localized = await _localizationService.Localize(LocalizationKeys.claim);
            _buttonText.SetText($"{localized} + {amount}");
        }
    }
}
using _Project.Scripts.Infrastructure.Services.Audio;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Buttons
{
    public class MultiplierButton : ButtonBase
    {
        [Inject] private AudioService _audioService;

        [SerializeField] private TextMeshProUGUI _multiplierText;
        [SerializeField] private TextMeshProUGUI _text;

        private int _multiplier;

        public void SetMultiplier(int multiplier)
        {
            _multiplier = multiplier;
            _multiplierText.SetText($"x{multiplier}");
        }

        public void SetAmount(int amount) => _text.SetText($"Получить + {amount * _multiplier}");

        protected override void OnClick() { }
    }
}
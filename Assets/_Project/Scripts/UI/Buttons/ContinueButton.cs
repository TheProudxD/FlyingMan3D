using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Tools.Extensions;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Buttons
{
    public class ContinueButton : ButtonBase
    {
        [Inject] private AudioService _audioService;

        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Image _adsImage;

        public void SetPriceState(string price)
        {
            _adsImage.Deactivate();
            _priceText.transform.parent.Activate();
            _priceText.text = price;
        }

        public void SetADsState()
        {
            _priceText.transform.parent.Deactivate();
            _adsImage.Activate();
        }

        protected override void OnClick() { }
    }
}
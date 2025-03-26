using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools.Extensions;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Views
{
    public class HintView : BaseView
    {
        [Inject] private HintService _hintService;
        [Inject] private WindowService _windowService;
        [Inject] private AnimationService _animationService;
        [Inject] private AdsService _adsService;
        [Inject] private MetricService _metricService;
        [Inject] private HintResourceService _hintResourceService;

        [SerializeField] private bool _interactable = true;

        [SerializeField] [ShowIf(nameof(_interactable))]
        private Button _button;

        [SerializeField] [ShowIf(nameof(_interactable))]
        private Image _adImage;
        
        private void Awake()
        {
            int value = _hintService.HintsNumber.Value;
            OnHintChanged(value, value);
        }

        private void OnEnable()
        {
            _hintService.HintsNumber.ChangedWithOld += OnHintChanged;
            int value = _hintService.HintsNumber.Value;
            OnHintChanged(value, value);

            if (_interactable)
                _button.Add(GetHint);
        }

        private void OnDisable()
        {
            _hintService.HintsNumber.ChangedWithOld -= OnHintChanged;

            if (_interactable)
                _button.Remove(GetHint);
        }

        private void OnHintChanged(int old, int @new)
        {
            _animationService.ResourceChanged(transform, old, @new, IncrementDuration,
                x => Text.SetText(x.ToString()));

            DisplayInfo();
        }

        private void GetHint()
        {
            if (_hintService.HintsNumber.Value <= 0 && _hintService.InHintMode == false)
            {
                _adsService.PlayRewardedVideo("hintInGame", UseHintForAds);
            }
            else
            {
                UseHint();
            }
        }

        private void UseHintForAds()
        {
            _hintResourceService.Add(this, 1);
            UseHint();
            //_metricService.HintForAdsInLevelUsed();
        }

        private void UseHint()
        {
            if (_hintService.UseHint(() => _button.interactable = true))
            {
                DisplayInfo();
                _button.interactable = false;
            }
            else
            {
                Debug.Log("Can not use hint");
            }
        }

        private void DisplayInfo()
        {
            if (_hintService.HintsNumber.Value <= 0 && _interactable)
            {
                Text.gameObject.SetActive(false);
                _adImage.gameObject.SetActive(true);
            }
            else
            {
                Text.gameObject.SetActive(true);

                if (_interactable)
                    _adImage.gameObject.SetActive(false);
            }
        }
    }
}
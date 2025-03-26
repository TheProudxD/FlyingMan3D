using System;
using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using YG;

namespace _Project.Scripts.UI.Windows
{
    public class StatisticsWindow : WindowBase
    {
        [Inject] private MetricService _metricService;
        [Inject] private AnimationService _animationService;

        [SerializeField] private Transform _popup;
        [SerializeField] private TextMeshProUGUI _fruitsMergedNumberText;
        [SerializeField] private TextMeshProUGUI _watermelonGuessedNumberText;
        [SerializeField] private TextMeshProUGUI _richestScoreNumberText;
        [SerializeField] private TextMeshProUGUI _gamesPlayedNumberText;

        private readonly float _fadeOutDuration = 0.13f;
        private readonly float _fadeInDuration = 0.15f;
        
        public override void Show()
        {
            base.Show();
            _animationService.FadeOut(_popup.gameObject, _fadeOutDuration);
            _metricService.StatisticsViewed();
        }

        public override void Hide()
        {
            _animationService.FadeIn(_popup.gameObject, _fadeInDuration, callback: () => base.Hide());
        }
    }
}
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Review;
using LitMotion;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public class ReviewThanksWindow : WindowBase
    {
        [Inject] private AnimationService _animationService;
        [Inject] private ConfigService _configService;
        
        [SerializeField] private TextMeshProUGUI _amountRewardText;
        [SerializeField] private Transform _popup;
        
        public override void Show()
        {
            base.Show();
            var reviewData = _configService.Get<ReviewData>();
            _amountRewardText.SetText(reviewData.Reward.ToString());
            _animationService.FadeOut(_popup.gameObject, 0.13f, Ease.OutBounce);
        }

        public override void Hide()
        {
            base.Hide();
            _animationService.FadeIn(_popup.gameObject, 0.15f, callback: () => base.Hide());
        }
    }
}
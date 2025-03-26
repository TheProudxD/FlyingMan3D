using _Project.Scripts.Infrastructure.Services;
using LitMotion;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public class LeaderboardWindow : WindowBase
    {
        [Inject] private AnimationService _animationService;
        
        [SerializeField] private Transform _popup;

        private readonly float _fadeOutDuration = 0.75f;
        private readonly float _fadeInDuration = 0.2f;
        
        public override void Show()
        {
            base.Show();

            _animationService.FadeOut(_popup.gameObject, _fadeOutDuration, Ease.OutBounce);
        }

        public override void Hide()
        {
            _animationService.FadeIn(_popup.gameObject, _fadeInDuration, callback: () => base.Hide());
        }
    }
}
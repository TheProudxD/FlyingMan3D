using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public class HintWindow : WindowBase
    {
        [Inject] private AnimationService _animationService;
        
        [SerializeField] private Transform _popup;

        private readonly float _fadeOutDuration = 0.4f;
        private readonly float _fadeInDuration = 0.4f;

        public override void Show()
        {
            base.Show();
            _animationService.FadeOut(_popup.gameObject, _fadeOutDuration);
        }

        public override void Hide()
        {
            base.Hide();
            _animationService.FadeIn(_popup.gameObject, _fadeInDuration);
        }
    }
}
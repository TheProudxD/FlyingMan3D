using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI.Buttons;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows
{
    public class PauseWindow : WindowBase
    {
        [Inject] private AnimationService _animationService;

        [SerializeField] private Transform _popup;
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private RestartGameButton _restartButton;
        [SerializeField] private MoreGamesButton _moreGamesButton;

        private readonly float _fadeOutDuration = 0.13f;
        private readonly float _fadeInDuration = 0.15f;

        public override void Show()
        {
            base.Show();
            _animationService.FadeOut(_popup.gameObject, _fadeOutDuration);

            Time.timeScale = 0;
            _continueGameButton.Add(ContinueGame);
            _restartButton.Activate();
            _restartButton.Add(Hide);
            _moreGamesButton.Activate();
        }

        private void ContinueGame()
        {
            AudioService.PlayClickSound();
            Hide();
        }
        
        public override void Hide()
        {
            _continueGameButton.Remove(ContinueGame);
            _restartButton.Remove(Hide);
            _restartButton.Deactivate();
            _moreGamesButton.Deactivate();

            Time.timeScale = 1;

            _animationService.FadeIn(_popup.gameObject, _fadeInDuration, callback: () => base.Hide());
        }
    }
}
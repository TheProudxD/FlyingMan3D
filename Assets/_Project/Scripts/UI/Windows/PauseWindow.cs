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
        [SerializeField] private CustomToggle _soundButton;
        //[SerializeField] private CustomToggle _musicButton;

        private readonly float _fadeOutDuration = 0.13f;
        private readonly float _fadeInDuration = 0.15f;

        public override void Show()
        {
            base.Show();
            _animationService.FadeOut(_popup.gameObject, _fadeOutDuration);

            Time.timeScale = 0;
            
            _soundButton.OnValueChanged += AmendSound;
            //_musicButton.OnValueChanged += AmendMusic;
            _continueGameButton.Add(ContinueGame);
            _restartButton.Activate();
            _restartButton.Add(Hide);
            _moreGamesButton.Activate();
        }

        private void AmendSound(bool isOn)
        {
            AudioService.PlayClickSound();

            if (isOn)
            {
                AudioService.UnmuteSound();
            }
            else
            {
                AudioService.MuteSound();
            }
        }

        private void ContinueGame()
        {
            AudioService.PlayClickSound();
            Hide();
        }
        
        private void AmendMusic(bool isOn)
        {
            AudioService.PlayClickSound();

            if (isOn)
            {
                AudioService.UnmuteMusic();
            }
            else
            {
                AudioService.MuteMusic();
            }
        }

        private void LoadSettings()
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                _soundButton.ToggleValue(false);
                AudioService.MuteSound();
            }
            else
            {
                _soundButton.ToggleValue(true);
                AudioService.UnmuteSound();
            }

            if (PlayerPrefs.GetInt("Music") == 0)
            {
                //_musicButton.ToggleValue(false);
                AudioService.MuteMusic();
            }
            else
            {
                //_musicButton.ToggleValue(true);
                AudioService.UnmuteMusic();
            }
        }
        
        public override void Hide()
        {
            _soundButton.OnValueChanged -= AmendSound;
            //_musicButton.OnValueChanged -= AmendMusic;
            _continueGameButton.Remove(ContinueGame);
            //_restartButton.Deactivate();
            _restartButton.Remove(Hide);
            //_openStatisticsButton.Deactivate();
            //_openLeaderboardButton.Deactivate();
            //_openMoreGamesButton.Deactivate();

            Time.timeScale = 1;
            
            _animationService.FadeIn(_popup.gameObject, _fadeInDuration, callback: () => base.Hide());
        }
    }
}
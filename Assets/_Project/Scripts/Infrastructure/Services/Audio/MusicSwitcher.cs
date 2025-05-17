using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.UI.Buttons;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public class MusicSwitcher : MonoBehaviour
    {
        [Inject] private AudioService _audioService;

        [SerializeField] private CustomToggle _musicToggle;

        private void OnEnable()
        {
            _musicToggle.ValueChanged += AmendMusic;
            _musicToggle.SetValue(true);
        }

        private void OnDisable()
        {
            _musicToggle.ValueChanged -= AmendMusic;
        }

        private void AmendMusic(bool isOn)
        {
            if (isOn)
            {
                _audioService.UnmuteMusic();
            }
            else
            {
                _audioService.MuteMusic();
            }
        }

        private void LoadSettings() => _musicToggle.SetValue(PlayerPrefs.GetInt("Music") != 0);
    }
}
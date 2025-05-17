using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.UI.Buttons;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Windows
{
    public class SoundSwitcher : MonoBehaviour
    {
        [Inject] private AudioService _audioService;

        [SerializeField] private CustomToggle _soundToggle;

        private void OnEnable()
        {
            _soundToggle.ValueChanged += AmendSound;
            _soundToggle.SetValue(true);
        }

        private void OnDisable()
        {
            _soundToggle.ValueChanged -= AmendSound;
        }

        private void AmendSound(bool isOn)
        {
            if (isOn)
            {
                _audioService.UnmuteSound();
            }
            else
            {
                _audioService.MuteSound();
            }
        }

        private void LoadSettings() => _soundToggle.SetValue(PlayerPrefs.GetInt("Sound") != 0);
    }
}
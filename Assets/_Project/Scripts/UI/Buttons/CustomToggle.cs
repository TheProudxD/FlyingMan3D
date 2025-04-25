using System;
using _Project.Scripts.Infrastructure.Services.Audio;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Buttons
{
    [RequireComponent(typeof(Toggle))]
    public class CustomToggle : MonoBehaviour
    {
        [Inject] private AudioService _audioService;
        
        [SerializeField] private Sprite _isOnSprite;
        [SerializeField] private Sprite _isOffSprite;

        private Toggle _toggle;

        public event Action<bool> ValueChanged;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(ToggleValue);
        }

        public void ToggleValue(bool value)
        {
            _audioService.PlayClickSound();
            _toggle.image.sprite = value ? _isOnSprite : _isOffSprite;
            ValueChanged?.Invoke(_toggle.isOn);
        }
    }
}
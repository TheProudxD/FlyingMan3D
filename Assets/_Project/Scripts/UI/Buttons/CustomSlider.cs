using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Buttons
{
    [RequireComponent(typeof(Slider))]
    public class CustomSlider : MonoBehaviour
    {
        private Slider _slider;

        public event Action<float> ValueChanged;

        private void Awake() => _slider = GetComponent<Slider>();

        private void OnEnable() => _slider.onValueChanged.AddListener(OnValueChanged);

        private void OnDisable() => _slider.onValueChanged.RemoveListener(OnValueChanged);

        private void OnValueChanged(float value) => ValueChanged?.Invoke(value);

        public void Initialize(float defaultValue) => _slider.value = defaultValue;
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Infrastructure.Services.Localization.UI
{
    public class LanguageButton : MonoBehaviour
    {
        public Action<LanguageButton, Locale> Pressed;

        [Header("Inner")]
        [SerializeField] private TextMeshProUGUI _label;

        private Button _button;
        private Locale _locale;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(Button_OnClick);
        }

        public void SetLocale(LocaleConfig locale)
        {
            _locale = locale.Code;
            _label.text = locale.Name;
        }

        private void Button_OnClick()
        {
            Pressed?.Invoke(this, _locale);
        }
    }
}
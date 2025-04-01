using System;
using System.Text.RegularExpressions;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Localization.SO;
using UnityEngine;
using YG;

namespace _Project.Scripts.Infrastructure.Services.Localization
{
    public class LocalizationService : IService
    {
        private readonly AssetProvider _assetProvider;
        private LocaleConfig _current;
        private TextCollection _currentCollection;

        public Action<LocaleConfig, int> LocaleChanged;

        public int UpdateNumber { get; private set; }

        public LocalizationService(AssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            YG2.onSwitchLang += SwitchLanguage;
        }

        ~LocalizationService() => YG2.onSwitchLang -= SwitchLanguage;

        private void SwitchLanguage(string lang)
        {
            switch (lang)
            {
                case "ru":
                    SetLocale(Locale.ru);
                    break;
                case "es":
                    SetLocale(Locale.es);
                    break;
                default:
                    SetLocale(Locale.en);
                    break;
            }
        }

        public LocaleConfig Current
        {
            get
            {
                if (_current == null)
                    SetDefaultLocale();

                return _current;
            }
        }

        public void SetLocale(SystemLanguage systemLanguage) => UpdateLocale(LocaleSettings.GetLocale(systemLanguage));

        public void SetLocale(Locale locale) => UpdateLocale(LocaleSettings.Locales[locale]);

        public void SetDefaultLocale() => UpdateLocale(LocaleSettings.GetDefault());

        private void UpdateLocale(LocaleConfig localeConfig)
        {
            if (localeConfig == null)
                return;

            _current = localeConfig;
            UpdateNumber++;

            _currentCollection = _assetProvider.GetLocal(localeConfig);

            LocaleChanged?.Invoke(_current, UpdateNumber);
        }

        public string Localize(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (_currentCollection == null)
            {
                return null;
            }

            if (Current == null)
            {
                UpdateLocale(LocaleSettings.GetDefault());
            }

            if (_currentCollection == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogError("Current collection is null");
#endif

                return "";
            }

            string text = _currentCollection.Localize(key);

            if (string.IsNullOrEmpty(text))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogError("No value for key: " + key);
#endif
            }

            return Regex.Unescape(text ?? string.Empty);
        }

        public string LocalizeUpper(string key)
        {
            string text = Localize(key);
            return string.IsNullOrEmpty(text) ? null : text.ToUpper();
        }

        public void DefineLanguage() => SwitchLanguage(YG2.lang);
    }
}
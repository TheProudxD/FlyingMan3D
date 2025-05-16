using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Localization.SO;
using UnityEngine;
using YG;

namespace _Project.Scripts.Infrastructure.Services.Localization
{
    public class LocalizationService : IService
    {
        private readonly AssetProvider _assetProvider;
        private TextCollection _currentCollection;

        public Action<LocaleConfig, int> LocaleChanged;

        public int UpdateNumber { get; private set; }
        public LocaleConfig Current { get; private set; }

        public LocalizationService(AssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            YG2.onSwitchLang += SwitchLanguage;
        }

        ~LocalizationService() => YG2.onSwitchLang -= SwitchLanguage;

        private async void SwitchLanguage(string lang)
        {
            bool tryParse = Enum.TryParse(lang, out Locale @case);

            await SetLocale(tryParse ? @case : Locale.en);
        }

        public Task SetLocale(SystemLanguage systemLanguage) => UpdateLocale(LocaleSettings.GetLocale(systemLanguage));

        public Task SetLocale(Locale locale) => UpdateLocale(LocaleSettings.Locales[locale]);

        public Task SetDefaultLocale() => UpdateLocale(LocaleSettings.GetDefault());

        private async Task UpdateLocale(LocaleConfig localeConfig)
        {
            if (localeConfig == null)
                return;

            Current = localeConfig;
            UpdateNumber++;

            _currentCollection = await _assetProvider.GetLocal(localeConfig);

            LocaleChanged?.Invoke(Current, UpdateNumber);
        }

        public async Task<string> Localize(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (Current == null)
            {
                await SetDefaultLocale();
            }
            
            if (_currentCollection == null)
            {
                _currentCollection = await _assetProvider.GetLocal(Current);
                return null;
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

        public async Task<string> LocalizeUpper(string key)
        {
            string text = await Localize(key);
            return string.IsNullOrEmpty(text) ? null : text.ToUpper();
        }

        public void DefineLanguage() => SwitchLanguage(YG2.lang);
    }
}
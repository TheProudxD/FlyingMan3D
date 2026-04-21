using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Localization
{
    public static class LocaleSettings
    {
        public static readonly Dictionary<Locale, LocaleConfig> Locales = new()
        {
            {
                Locale.en, new LocaleConfig
                {
                    Name = "English",
                    Code = Locale.en,
                    SystemLanguages = new[]
                    {
                        SystemLanguage.English
                    },
                    DateFormat = "dd-MM-yyyy",
                    DateSeparator = "/",
                    DateEndianness = LocaleConfig.EndiannessType.Little,
                    ReadingDirection = LanguageReadingDirection.LeftToRight
                }
            },
            {
                Locale.ru, new LocaleConfig
                {
                    Name = "Russian",
                    Code = Locale.ru,
                    SystemLanguages = new[]
                    {
                        SystemLanguage.Russian
                    },
                    DateFormat = "dd-MM-yyyy",
                    DateSeparator = "/",
                    DateEndianness = LocaleConfig.EndiannessType.Little,
                    ReadingDirection = LanguageReadingDirection.LeftToRight
                }
            },
            {
                Locale.es, new LocaleConfig
                {
                    Name = "Spanish",
                    Code = Locale.es,
                    SystemLanguages = new[]
                    {
                        SystemLanguage.Spanish
                    },
                    DateFormat = "dd-MM-yyyy",
                    DateSeparator = "/",
                    DateEndianness = LocaleConfig.EndiannessType.Little,
                    ReadingDirection = LanguageReadingDirection.LeftToRight
                }
            },
            {
                Locale.de, new LocaleConfig()
                {
                    Name = "German",
                    Code = Locale.de,
                    SystemLanguages = new[]
                    {
                        SystemLanguage.German
                    },
                    DateFormat = "dd-MM-yyyy",
                    DateSeparator = "/",
                    DateEndianness = LocaleConfig.EndiannessType.Little,
                    ReadingDirection = LanguageReadingDirection.LeftToRight
                }
            }
        };

        public static LocaleConfig GetLocale(SystemLanguage systemLanguage)
        {
            foreach (LocaleConfig localeConfig in Locales.Values)
            {
                SystemLanguage[] systemLanguages = localeConfig.SystemLanguages;

                if (systemLanguages == null)
                    continue;

                for (int i = 0; i < systemLanguages.Length; i++)
                {
                    if (systemLanguages[i] == systemLanguage)
                        return localeConfig;
                }
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.LogWarning("No locale for " + systemLanguage);
#endif

            LocaleConfig fallbackLocale = GetDefault();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.Log("Using default locale " + fallbackLocale);
#endif

            return fallbackLocale;
        }
        
        public static LocaleConfig GetDefault() => Locales[Locale.en];

        public static List<LocaleConfig> GetActiveLocales() =>
            Locales.Where(i => i.Value.IsEnabled).Select(i => i.Value).ToList();
    }
}

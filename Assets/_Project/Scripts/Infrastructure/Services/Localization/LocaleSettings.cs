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
                    Name = "Russian",
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
            LocaleConfig localeConfig =
                Locales.FirstOrDefault(l => l.Value.SystemLanguages.Contains(systemLanguage)).Value;

            if (localeConfig == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogWarning("No locale for " + systemLanguage);
#endif

                localeConfig = GetDefault();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.Log("Using default locale " + localeConfig);
#endif
            }

            return localeConfig;
        }
        
        public static LocaleConfig GetDefault() => Locales[Locale.en];

        public static List<LocaleConfig> GetActiveLocales() =>
            Locales.Where(i => i.Value.IsEnabled).Select(i => i.Value).ToList();
    }
}
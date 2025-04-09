using System.Text;
using _Project.Scripts.Tools.Extensions;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Localization
{
    /// <summary>
    /// All the locales supported by the application.
    /// https://en.wikipedia.org/wiki/Locale_(computer_software)
    /// https://www.science.co.il/language/Locale-codes.php
    /// </summary>
    public enum Locale
    {
        NotDefined,
        en,
        ru,
        es,
        de
    }

    public class LocaleConfig
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Endianness#Endian_dates
        /// </summary>
        public enum EndiannessType
        {
            Big,
            Little,
            Middle
        }

        private const string TO_STRING_FORMAT = "{0} : {1}";

        public int ServerId { get; set; } = -1; // assigned at runtime
        public string Name { get; set; }
        public string DateFormat { get; set; }
        public string DateSeparator { get; set; }
        public string ISO => Code.ToString();
        public EndiannessType DateEndianness { get; set; }
        public Locale Code { get; set; }
        public SystemLanguage[] SystemLanguages { get; set; }
        public string ThemeName { get; set; }

        public bool IsEnabled => ServerId != -1;
        public LanguageReadingDirection ReadingDirection { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TO_STRING_FORMAT, "Name", Name);
            builder.AppendFormat(TO_STRING_FORMAT, "Date Format", DateFormat);
            builder.AppendFormat(TO_STRING_FORMAT, "Locale", Code);
            builder.AppendFormat(TO_STRING_FORMAT, "ISO", ISO);
            builder.AppendFormat(TO_STRING_FORMAT, "System Languages", SystemLanguages.Print());
            builder.AppendFormat(TO_STRING_FORMAT, "Code", Code.ToString());
            builder.AppendFormat(TO_STRING_FORMAT, "Theme", ThemeName);
            builder.AppendFormat(TO_STRING_FORMAT, "ReadingDirection", ReadingDirection.ToString());
            return builder.ToString();
        }

        public static Locale GetLocale(string ietfTag)
        {
            // Replace char to match with enum
            ietfTag = ietfTag.Replace('-', '_');

            // Return locale or NotDefined.
            return ietfTag.FromString(Locale.NotDefined);
        }

        public static string GetIeftTag(Locale locale) => locale.ToString().Replace('_', '-');
    }
}
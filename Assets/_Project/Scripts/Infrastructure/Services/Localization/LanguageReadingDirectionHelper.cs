namespace _Project.Scripts.Infrastructure.Services.Localization
{
    public static class LanguageReadingDirectionHelper
    {
        public static string ToString(LanguageReadingDirection languageReadingDirection) =>
            languageReadingDirection switch
            {
                LanguageReadingDirection.LeftToRight => "ltr",
                LanguageReadingDirection.RightToLeft => "rtl",
                _ => null
            };
    }
}
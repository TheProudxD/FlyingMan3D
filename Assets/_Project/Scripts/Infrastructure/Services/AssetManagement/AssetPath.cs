using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.AssetManagement
{
    public static class AssetPath
    {
        private const string DATA_FOLDER = "Data";
        private const string GRAPHICS_FOLDER = "Graphics";
        private const string LOCALE_FOLDER = "Locales";
        private const string GAMEPLAY_FOLDER = "Gameplay";

        public const string LOCALE_RESOURCES_PATH_FORMAT = LOCALE_FOLDER + "/{0}";

        public const string CONFIG_CONTAINER = DATA_FOLDER + "/ConfigContainer";
        public const string WINDOW_CONFIG_PATH = DATA_FOLDER + "/UI/WindowsData";

        public const string AUDIO_SERVICE_VIEW = "Audio";

        public const string MOBILE_GRAPHICS_PRESET = GRAPHICS_FOLDER + "/MobileGraphics";
        public const string DESKTOP_GRAPHICS_PRESET = GRAPHICS_FOLDER + "/DesktopGraphics";

        public const string POST_PROCESS_VOLUME = "PostProcessVolume";

        public const string SUCCESS_ICON = GAMEPLAY_FOLDER + "/SuccessIcon";
        public const string MISS_ICON = GAMEPLAY_FOLDER + "/MissIcon";
    }
}
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services
{
    public class DeviceSpecificGraphics : IService
    {
        private readonly AssetProvider _assetProvider;

        public DeviceSpecificGraphics(AssetProvider assetProvider) => _assetProvider = assetProvider;

        public async UniTask DefineGraphicsSettings()
        {
            GraphicsPreset preset = await (IsMobileDevice()
                ? _assetProvider.GetMobileGraphicsPreset()
                : _assetProvider.GetDesktopGraphicsPreset());

            QualitySettings.globalTextureMipmapLimit = preset.textureQuality;
            QualitySettings.shadows = preset.shadows;
            QualitySettings.shadowResolution = preset.shadowResolution;
            QualitySettings.shadowDistance = preset.shadowDistance;
            QualitySettings.antiAliasing = preset.antiAliasing;
            Application.targetFrameRate = preset.targetFPS;
            QualitySettings.vSyncCount = preset.vSync ? 1 : 0;
            QualitySettings.lodBias = preset.lodBias;
            QualitySettings.realtimeReflectionProbes = preset.realtimeReflectionProbes;
            QualitySettings.softParticles = preset.softParticles;
            QualitySettings.anisotropicFiltering = preset.anisotropicFiltering;

            if (preset.postProcessing && !SystemInfo.supportsComputeShaders)
            {
                preset.postProcessing = false;
            }

            // if (preset.postProcessing)
            // {
            //     _assetProvider.CreatePostProcessVolume();
            // }

            ConfigurePlatformSpecificFeatures();
        }

        private void ConfigurePlatformSpecificFeatures()
        {
            if (IsMobileDevice())
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        private bool IsMobileDevice()
        {
            if (Application.isMobilePlatform)
                return true;

            return SystemInfo.deviceType == DeviceType.Handheld;
        }

        private bool IsDesktop()
        {
            if (Application.isEditor)
                return true;

            return SystemInfo.deviceType == DeviceType.Desktop;
        }

        private bool PerformanceIsLow() => 1.0f / Time.deltaTime < 30; // FPS ниже 30

        private void ReduceQualityLevel()
        {
            int currentLevel = QualitySettings.GetQualityLevel();

            if (currentLevel > 0)
            {
                QualitySettings.SetQualityLevel(currentLevel - 1);
            }
        }

        private void AdjustResolution()
        {
            int maxResolution = 1080;

            if (IsMobileDevice())
            {
                maxResolution = Screen.currentResolution.height;
                maxResolution = (int)Mathf.Min(maxResolution, QualitySettings.resolutionScalingFixedDPIFactor);
            }

            Screen.SetResolution((int)(maxResolution * 0.8f), (int)(maxResolution * 0.45f), true);
        }
    }
}

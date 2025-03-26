using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services
{
    [CreateAssetMenu(menuName = "Configs GraphicsPreset", fileName = "GraphicsPreset", order = 0)]
    public class GraphicsPreset : Config.Config
    {
        public int textureQuality; // 0=полное качество, 3=самое низкое
        public ShadowQuality shadows;
        public ShadowResolution shadowResolution; // Разрешение теней
        public float shadowDistance; // Дистанция отрисовки теней
        public bool postProcessing;
        public int antiAliasing; // 0=off, 2=MSAAx2, 4=MSAAx4
        public int targetFPS;
        public bool vSync;
        public float lodBias;
        public bool realtimeReflectionProbes;
        public bool softParticles;
        public AnisotropicFiltering anisotropicFiltering;
    }
}
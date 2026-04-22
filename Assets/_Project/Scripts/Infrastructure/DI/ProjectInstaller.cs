using _Project.Scripts.Tools.Camera;
using _Project.Scripts.UI;
using Reflex.Core;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.DI
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private CameraSetup _cameraSetup;
        [SerializeField] private LoadingCurtain _loadingCurtain;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.BindProjectAdsAndPlatform();
            builder.BindProjectProgress();
            builder.BindProjectCore(_cameraSetup, _loadingCurtain);
            builder.BindProjectUi();
            builder.BindProjectGameplay();
        }
    }
}

using Reflex.Attributes;
using UnityEngine;
using _Project.Scripts.Tools.Camera;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CameraBinder : MonoBehaviour
    {
        [Inject] private CameraSetup _cameraSetup;

        private void Awake() => GetComponent<Canvas>().worldCamera = _cameraSetup.MainCamera;
    }
}
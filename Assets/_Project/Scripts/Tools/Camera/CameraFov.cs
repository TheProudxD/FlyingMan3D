/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;

namespace _Project.Scripts.Tools.Camera
{
    public class CameraFov : MonoBehaviour {

        private UnityEngine.Camera _playerCamera;
        private float _targetFov;
        private float _fov;

        private void Awake() {
            _playerCamera = GetComponent<UnityEngine.Camera>();
            _targetFov = _playerCamera.fieldOfView;
            _fov = _targetFov;
        }

        private void Update() {
            float fovSpeed = 4f;
            _fov = Mathf.Lerp(_fov, _targetFov, Time.deltaTime * fovSpeed);
            _playerCamera.fieldOfView = _fov;
        }

        public void SetCameraFov(float targetFov) {
            _targetFov = targetFov;
        }
    }
}

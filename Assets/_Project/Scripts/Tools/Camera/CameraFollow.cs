/* 
    ------------------- Code Monkey -------------------
    
    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using UnityEngine;

namespace _Project.Scripts.Tools.Camera {

    /*
     * Script to handle Camera Movement and Zoom
     * Place on Camera GameObject
     * */
    public class CameraFollow : MonoBehaviour {

        private UnityEngine.Camera _myCamera;
        private Func<Vector3> _getCameraFollowPositionFunc;
        private Func<float> _getCameraZoomFunc;

        public void Setup(Func<Vector3> getCameraFollowPositionFunc, Func<float> getCameraZoomFunc, bool teleportToFollowPosition, bool instantZoom) {
            _getCameraFollowPositionFunc = getCameraFollowPositionFunc;
            _getCameraZoomFunc = getCameraZoomFunc;

            if (teleportToFollowPosition) {
                Vector3 cameraFollowPosition = getCameraFollowPositionFunc();
                cameraFollowPosition.z = transform.position.z;
                transform.position = cameraFollowPosition;
            }

            if (instantZoom) {
                _myCamera.orthographicSize = getCameraZoomFunc();
            }
        }

        private void Awake() {
            _myCamera = transform.GetComponent<UnityEngine.Camera>();
        }

        public void SetCameraFollowPosition(Vector3 cameraFollowPosition) {
            SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
        }

        public void SetGetCameraFollowPositionFunc(Func<Vector3> getCameraFollowPositionFunc) {
            _getCameraFollowPositionFunc = getCameraFollowPositionFunc;
        }

        public void SetCameraZoom(float cameraZoom) {
            SetGetCameraZoomFunc(() => cameraZoom);
        }

        public void SetGetCameraZoomFunc(Func<float> getCameraZoomFunc) {
            _getCameraZoomFunc = getCameraZoomFunc;
        }


        private void Update() {
            HandleMovement();
            HandleZoom();
        }

        private void HandleMovement() {
            if (_getCameraFollowPositionFunc == null) return;
            Vector3 cameraFollowPosition = _getCameraFollowPositionFunc();
            cameraFollowPosition.z = transform.position.z;

            Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
            float distance = Vector3.Distance(cameraFollowPosition, transform.position);
            float cameraMoveSpeed = 3f;

            if (distance > 0) {
                Vector3 newCameraPosition = transform.position + cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime;

                float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);

                if (distanceAfterMoving > distance) {
                    // Overshot the target
                    newCameraPosition = cameraFollowPosition;
                }

                transform.position = newCameraPosition;
            }
        }

        private void HandleZoom() {
            if (_getCameraZoomFunc == null) return;
            float cameraZoom = _getCameraZoomFunc();

            float cameraZoomDifference = cameraZoom - _myCamera.orthographicSize;
            float cameraZoomSpeed = 1f;

            _myCamera.orthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

            if (cameraZoomDifference > 0) {
                if (_myCamera.orthographicSize > cameraZoom) {
                    _myCamera.orthographicSize = cameraZoom;
                }
            } else {
                if (_myCamera.orthographicSize < cameraZoom) {
                    _myCamera.orthographicSize = cameraZoom;
                }
            }
        }
    }

}
using System;
using UnityEngine;

namespace _Project.Scripts.Tools.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraFollow : MonoBehaviour
    {
        private const float DEFAULT_MOVE_SPEED = 3f;
        
        private UnityEngine.Camera _myCamera;
        private Func<Vector3> _getCameraFollowPositionFunc;
        private Func<float> _getCameraZoomFunc;
        private float _moveSpeed;
        private bool _followPosition = true;
        private bool _followRotation = false;
        private bool _useFixedUpdate = false;
        private float _distanceThreshold = 0.1f;
        
        public void Setup(Func<Vector3> getCameraFollowPositionFunc, Func<float> getCameraZoomFunc = null,
            float moveSpeed = DEFAULT_MOVE_SPEED,
            bool teleportToFollowPosition = false, bool instantZoom = false)
        {
            _getCameraFollowPositionFunc = getCameraFollowPositionFunc;
            _getCameraZoomFunc = getCameraZoomFunc;
            _moveSpeed = moveSpeed;

            if (teleportToFollowPosition)
            {
                Vector3 cameraFollowPosition = getCameraFollowPositionFunc();
                cameraFollowPosition.z = transform.position.z;
                transform.position = cameraFollowPosition;
            }

            if (instantZoom)
            {
                if (getCameraZoomFunc != null)
                    _myCamera.orthographicSize = getCameraZoomFunc();
            }
        }

        private void Awake() => _myCamera = GetComponent<UnityEngine.Camera>();

        public void SetCameraFollowPosition(Vector3 cameraFollowPosition)
        {
            SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
        }

        public void SetGetCameraFollowPositionFunc(Func<Vector3> getCameraFollowPositionFunc)
        {
            _getCameraFollowPositionFunc = getCameraFollowPositionFunc;
        }

        public void SetCameraZoom(float cameraZoom)
        {
            SetGetCameraZoomFunc(() => cameraZoom);
        }

        public void SetGetCameraZoomFunc(Func<float> getCameraZoomFunc)
        {
            _getCameraZoomFunc = getCameraZoomFunc;
        }

        public void SetMoveSpeed(float moveSpeed) => _moveSpeed = moveSpeed;

        private void LateUpdate()
        {
            if (!_useFixedUpdate)
            {
                HandleMovement();
                HandleZoom();
            }
        }

        private void FixedUpdate()
        {
            if (_useFixedUpdate)
            {
                HandleMovement();
                HandleZoom();
            }
        }

        private void HandleMovement()
        {
            if (_getCameraFollowPositionFunc == null) return;

            Vector3 desiredPosition = _getCameraFollowPositionFunc();

            if (_followPosition)
            {
                if (Vector3.Distance(transform.position, desiredPosition) > _distanceThreshold)
                {
                    transform.position = Vector3.Lerp(
                        transform.position,
                        desiredPosition,
                        _moveSpeed * Time.deltaTime
                    );
                }
            }

            if (_followRotation)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(
                    desiredPosition - transform.position,
                    Vector3.up
                );

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    desiredRotation,
                    _moveSpeed * Time.deltaTime
                );
            }
        }

        // private void HandleMovement()
        // {
        //     if (_getCameraFollowPositionFunc == null) return;
        //
        //     Vector3 cameraFollowPosition = _getCameraFollowPositionFunc();
        //     // cameraFollowPosition.z = transform.position.z;
        //
        //     Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        //     float distance = Vector3.Distance(cameraFollowPosition, transform.position);
        //
        //     if (_moveSpeed <= 0)
        //     {
        //         transform.position = cameraFollowPosition;
        //     }
        //     else
        //     {
        //         if (distance <= 0)
        //             return;
        //
        //         Vector3 newCameraPosition =
        //             transform.position + cameraMoveDir * distance * _moveSpeed * Time.fixedDeltaTime;
        //
        //         float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);
        //
        //         if (distanceAfterMoving > distance)
        //         {
        //             // Overshot the target
        //             newCameraPosition = cameraFollowPosition;
        //         }
        //
        //         transform.position = newCameraPosition;
        //     }
        // }

        private void HandleZoom()
        {
            if (_getCameraZoomFunc == null) return;

            float cameraZoom = _getCameraZoomFunc();

            float cameraZoomDifference = cameraZoom - _myCamera.orthographicSize;
            float cameraZoomSpeed = 1f;

            _myCamera.orthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

            if (cameraZoomDifference > 0)
            {
                if (_myCamera.orthographicSize > cameraZoom)
                {
                    _myCamera.orthographicSize = cameraZoom;
                }
            }
            else
            {
                if (_myCamera.orthographicSize < cameraZoom)
                {
                    _myCamera.orthographicSize = cameraZoom;
                }
            }
        }
    }
}
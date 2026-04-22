using _Project.Scripts.Gameplay;
using _Project.Scripts.Tools.Camera;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Camera
{
    public sealed class CameraService : IService
    {
        private readonly CameraSetup _cameraSetup;

        public CameraService(CameraSetup cameraSetup) => _cameraSetup = cameraSetup;

        public void SetPlayerCamera(PlayerController player)
        {
            if (player == null || player.SelfHips == null || _cameraSetup == null)
                return;

            var offset = new Vector3(0, 7, -14);
            var follow = _cameraSetup.CameraFollow;

            follow.transform.position = player.SelfHips.transform.position + offset;
            follow.enabled = true;
            follow.transform.rotation = Quaternion.Euler(new Vector3(10, 0, 0));

            _cameraSetup.MainCamera.fieldOfView = 60;

            follow.Setup(() =>
            {
                if (player != null && player.SelfHips != null)
                {
                    follow.enabled = true;
                    return player.SelfHips.transform.position + offset;
                }

                follow.enabled = false;
                return Vector3.zero;
            }, moveSpeed: 500f);
        }

        public void SetFinishCamera(float finishZPosition)
        {
            if (_cameraSetup == null)
                return;

            var follow = _cameraSetup.CameraFollow;

            _cameraSetup.MainCamera.fieldOfView = 90;
            follow.SetMoveSpeed(-1);
            follow.enabled = false;
            follow.transform.position = new Vector3(0, 20, finishZPosition - 35f);
            follow.transform.rotation = Quaternion.Euler(new Vector3(50, 0, 0));
        }
    }
}

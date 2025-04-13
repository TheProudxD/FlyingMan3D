using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            _cameraTransform = GetComponent<Canvas>().worldCamera.transform;
        }

        private void Update()
        {
            transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
                _cameraTransform.rotation * Vector3.up);
        }
    }
}
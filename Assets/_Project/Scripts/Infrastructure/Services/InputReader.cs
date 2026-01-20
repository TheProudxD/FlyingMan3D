using UnityEngine;
using UnityEngine.EventSystems;
using _Project.Scripts.Tools.Camera;

namespace _Project.Scripts.Infrastructure.Services
{
    public class InputReader : IService
    {
        private const string LAYER_NAME = "Default";
        private readonly Camera _camera;

        public InputReader(CameraSetup cameraSetup) => _camera = cameraSetup.MainCamera;

        public Vector3 GetWorldPosition(Vector3 startMousePos) =>
            _camera.ViewportToWorldPoint(startMousePos);

        public Vector3 GetMousePosition(Vector3 position) => _camera.WorldToScreenPoint(position);

        public Vector3 GetPointerWorldPosition(PointerEventData eventData)
        {
            Ray ray = _camera.ScreenPointToRay(eventData.position);

            return Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask: LayerMask.GetMask(LAYER_NAME))
                ? hit.point
                : Vector3.zero;
        }
    }
}
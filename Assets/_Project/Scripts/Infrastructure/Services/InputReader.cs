using UnityEngine;
using UnityEngine.EventSystems;
using _Project.Scripts.Tools.Camera;
using System.Collections.Generic;

namespace _Project.Scripts.Infrastructure.Services
{
    public class InputReader : IService
    {
        private const string LAYER_NAME = "Default";
        private readonly Camera _camera;
        private readonly List<RaycastResult> _uiRaycastResults = new(8);

        public InputReader(CameraSetup cameraSetup) => _camera = cameraSetup.MainCamera;

        public bool GetMouseButton(int button) => Input.GetMouseButton(button);

        public bool GetMouseButtonDown(int button) => Input.GetMouseButtonDown(button);

        public float GetAxis(string axisName) => Input.GetAxis(axisName);

        public Vector3 GetPointerScreenPosition() => Input.mousePosition;

        public bool IsPointerOverUI()
        {
            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null)
                return false;

            var eventData = new PointerEventData(eventSystem)
            {
                position = GetPointerScreenPosition()
            };

            _uiRaycastResults.Clear();
            eventSystem.RaycastAll(eventData, _uiRaycastResults);
            return _uiRaycastResults.Count > 0;
        }

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

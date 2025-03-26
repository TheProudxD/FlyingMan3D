using UnityEngine;

namespace _Project.Scripts.Tools.Extensions
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Calculates and returns viewport extents with an optional margin. Useful for calculating a frustum for culling.
        /// </summary>
        /// <param name="camera">The camera object this method extends.</param>
        /// <param name="viewportMargin">Optional margin to be applied to viewport extents. Default is 0.2, 0.2.</param>
        /// <returns>Viewport extents as a Vector2 after applying the margin.</returns>
        public static Vector2 GetViewportExtentsWithMargin(this UnityEngine.Camera camera,
            Vector2? viewportMargin = null)
        {
            Vector2 margin = viewportMargin ?? new Vector2(0.2f, 0.2f);

            Vector2 result;
            float halfFieldOfView = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            result.y = camera.nearClipPlane * Mathf.Tan(halfFieldOfView);
            result.x = result.y * camera.aspect + margin.x;
            result.y += margin.y;
            return result;
        }

        public static Vector3 GetWorldPositionFromUIWithZeroZ(this UnityEngine.Camera camera)
        {
            var vec = GetWorldPositionFromUI(camera, Input.mousePosition);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetWorldPositionFromUI(this UnityEngine.Camera camera) =>
            GetWorldPositionFromUI(camera, Input.mousePosition);

        public static Vector3 GetWorldPositionFromUI(this UnityEngine.Camera camera, Vector3 screenPosition) =>
            camera.ScreenToWorldPoint(screenPosition);

        public static Vector3 GetWorldPositionFromUI_Perspective(this UnityEngine.Camera camera) =>
            GetWorldPositionFromUI_Perspective(camera, Input.mousePosition);

        public static Vector2 GetWorldUIPosition(this UnityEngine.Camera uiCamera, Vector3 worldPosition,
            Transform parent,
            UnityEngine.Camera worldCamera)
        {
            var screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            var uiCameraWorldPosition = uiCamera.ScreenToWorldPoint(screenPosition);
            var localPos = parent.InverseTransformPoint(uiCameraWorldPosition);
            return new Vector2(localPos.x, localPos.y);
        }

        public static Vector3 GetWorldPositionFromUI_Perspective(this UnityEngine.Camera worldCamera,
            Vector3 screenPosition)
        {
            var ray = worldCamera.ScreenPointToRay(screenPosition);
            var plane = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
            plane.Raycast(ray, out var distance);
            return ray.GetPoint(distance);
        }

        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition(this UnityEngine.Camera camera) =>
            GetMouseWorldPosition(camera, Input.mousePosition);

        public static Vector3 GetMouseWorldPosition(this UnityEngine.Camera camera, Vector3 position)
        {
            var vec = GetMouseWorldPositionWithZ(camera, position);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ(this UnityEngine.Camera camera) =>
            GetMouseWorldPositionWithZ(camera, Input.mousePosition);

        public static Vector3 GetMouseWorldPositionWithZ(this UnityEngine.Camera camera, Vector3 screenPosition) =>
            camera.ScreenToWorldPoint(screenPosition);

        public static void LayerCullingShow(this UnityEngine.Camera cam, int layerMask) => cam.cullingMask |= layerMask;

        public static void LayerCullingShow(this UnityEngine.Camera cam, string layer) =>
            LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));

        public static void LayerCullingHide(this UnityEngine.Camera cam, int layerMask) =>
            cam.cullingMask &= ~layerMask;

        public static void LayerCullingHide(this UnityEngine.Camera cam, string layer) =>
            LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));

        public static void LayerCullingToggle(this UnityEngine.Camera cam, int layerMask) =>
            cam.cullingMask ^= layerMask;

        public static void LayerCullingToggle(this UnityEngine.Camera cam, string layer) =>
            LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer));

        public static bool LayerCullingIncludes(this UnityEngine.Camera cam, int layerMask) =>
            (cam.cullingMask & layerMask) > 0;

        public static bool LayerCullingIncludes(this UnityEngine.Camera cam, string layer) =>
            LayerCullingIncludes(cam, 1 << LayerMask.NameToLayer(layer));

        public static void LayerCullingToggle(this UnityEngine.Camera cam, string layer, bool isOn) =>
            LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer), isOn);

        public static void LayerCullingToggle(this UnityEngine.Camera cam, int layerMask, bool isOn)
        {
            bool included = LayerCullingIncludes(cam, layerMask);

            if (isOn && !included)
            {
                LayerCullingShow(cam, layerMask);
            }
            else if (!isOn && included)
            {
                LayerCullingHide(cam, layerMask);
            }
        }
    }
}
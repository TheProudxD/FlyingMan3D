using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Tools.Camera
{
    [ExecuteAlways]
    public class CameraSetup : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _mainCamera;
        [SerializeField] private UnityEngine.Camera _postProccessingCamera;
        [SerializeField] private bool _autoFindCamera = false;
        [SerializeField] private float _offset = 0f;
        [SerializeField] private float _cameraAngle = 75f;
        [SerializeField] private float _cameraFov = 60f;
        [SerializeField] private bool _changeFovDynamically;

        public UnityEngine.Camera MainCamera => _mainCamera;
        public UnityEngine.Camera PostProccessingCamera => _postProccessingCamera;

        private void Awake()
        {
            if (Application.isPlaying)
                DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (!_changeFovDynamically)
                return;

            if (_autoFindCamera && _mainCamera == null)
            {
                _mainCamera = FindObjectOfType<UnityEngine.Camera>();
            }

            if (_postProccessingCamera == null || _mainCamera == null) return;

            _postProccessingCamera.fieldOfView = _cameraFov;
            _mainCamera.fieldOfView = _cameraFov;
            float cameraAngleResult = _cameraAngle;

            if (_cameraAngle > 90f)
                cameraAngleResult = 180f - _cameraAngle;

            Quaternion relativeRotation = Quaternion.Euler(cameraAngleResult, 0f, 0f);
            _postProccessingCamera.transform.rotation = transform.rotation * relativeRotation;
            _mainCamera.transform.rotation = transform.rotation * relativeRotation;

            float h = transform.localScale.z;
            float w = transform.localScale.x;
            float a = cameraAngleResult;
            a = Mathf.Deg2Rad * a;
            float f = Mathf.Deg2Rad * _postProccessingCamera.fieldOfView / 2f;

            // âû÷èñëåíèå ïîçèöèè, ÷òîáû âñå âëåçëî ïî âåðòèêàëè
            float x1 = h * Mathf.Tan(a - f) / (-Mathf.Tan(a - f) + Mathf.Tan(a + f));
            float y1 = x1 * Mathf.Tan(a + f);

            Vector3 vec = _postProccessingCamera.ViewportPointToRay(new Vector3(1f, 0, 1f)).direction;
            vec = InverseTransformDirectionIgnoringScale(vec);
            vec.z = 0f;

            float fProjection = Vector3.Angle(new Vector3(0, -1f, 0), vec);
            float y2 = w * 0.5f / Mathf.Tan(fProjection * Mathf.Deg2Rad);
            float x2 = y2 / Mathf.Tan(a + f);

            float startZ = -transform.localScale.z / 2f;
            float distance1 = x1 * x1 + y1 * y1;
            float distance2 = x2 * x2 + y2 * y2;

            if (distance1 > distance2)
            {
                _postProccessingCamera.transform.position =
                    TransformPointIgnoringScale(new Vector3(0f, y1, startZ - x1));

                _mainCamera.transform.position =
                    TransformPointIgnoringScale(new Vector3(0f, y1, startZ - x1));

                if (_cameraAngle > 90f)
                {
                    ReflectTransform(_postProccessingCamera.transform);
                    ReflectTransform(_mainCamera.transform);

                    _postProccessingCamera.transform.localEulerAngles =
                        new Vector3(180f - _postProccessingCamera.transform.localEulerAngles.x, 0f, 0f);

                    _mainCamera.transform.localEulerAngles =
                        new Vector3(180f - _mainCamera.transform.localEulerAngles.x, 0f, 0f);
                }
            }
            else
            {
                // Ïî ãîðèçîíòàëè
                float r1 = Mathf.Sqrt(x1 * x1 + y1 * y1);
                float r2 = Mathf.Sqrt(x2 * x2 + y2 * y2);

                float direction = 1f;

                _postProccessingCamera.transform.position =
                    TransformPointIgnoringScale(new Vector3(0f, y2, startZ - x2));

                _mainCamera.transform.position =
                    TransformPointIgnoringScale(new Vector3(0f, y2, startZ - x2));

                if (_cameraAngle > 90f)
                {
                    ReflectTransform(_postProccessingCamera.transform);
                    ReflectTransform(_mainCamera.transform);

                    _postProccessingCamera.transform.localEulerAngles =
                        new Vector3(180f - _postProccessingCamera.transform.localEulerAngles.x, 0f, 0f);

                    _mainCamera.transform.localEulerAngles =
                        new Vector3(180f - _mainCamera.transform.localEulerAngles.x, 0f, 0f);

                    direction = -1f;
                }

                _postProccessingCamera.transform.position +=
                    (r2 - r1) * _offset * direction * _postProccessingCamera.transform.up;

                _mainCamera.transform.position +=
                    (r2 - r1) * _offset * direction * _mainCamera.transform.up;
            }
        }

        private Vector3 TransformPointIgnoringScale(Vector3 point) => transform.position + transform.rotation * point;

        private Vector3 InverseTransformPointIgnoringScale(Vector3 point) =>
            Quaternion.Inverse(transform.rotation) * (point - transform.position);

        private Vector3 InverseTransformDirectionIgnoringScale(Vector3 worldDirection) =>
            Quaternion.Inverse(transform.rotation) * worldDirection;

        // Îòðàæåíèå ïîçèöèè
        private void ReflectTransform(Transform objectToReflect)
        {
            Vector3 localPos = InverseTransformPointIgnoringScale(objectToReflect.position);
            localPos.z = -localPos.z; // Îòðàæåíèå îòíîñèòåëüíî ïëîñêîñòè XY
            objectToReflect.position = TransformPointIgnoringScale(localPos);
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_changeFovDynamically == false)
                return;

            Vector3 a = transform.TransformPoint(-0.5f, 0, -0.5f);
            Vector3 b = transform.TransformPoint(-0.5f, 0, 0.5f);
            Vector3 c = transform.TransformPoint(0.5f, 0, 0.5f);
            Vector3 d = transform.TransformPoint(0.5f, 0, -0.5f);

            Handles.color = Color.black;
            float width = 4;
            Handles.DrawLine(a, b, width);
            Handles.DrawLine(b, c, width);
            Handles.DrawLine(c, d, width);
            Handles.DrawLine(d, a, width);
        }
#endif
    }
}
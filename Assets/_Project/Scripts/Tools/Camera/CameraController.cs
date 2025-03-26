using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Tools.Camera
{

	/// <summary>
	/// 3rd person camera controller.
	/// </summary>
	public class CameraController : MonoBehaviour
	{

		// When to update the camera?
		[System.Serializable]
		public enum UpdateMode
		{
			Update,
			FixedUpdate,
			LateUpdate,
			FixedLateUpdate
		}

		[FormerlySerializedAs("target")] public Transform Target; // The target Transform to follow
		[FormerlySerializedAs("rotationSpace")] public Transform RotationSpace; // If assigned, will use this Transform's rotation as the rotation space instead of the world space. Useful with spherical planets.
		public UpdateMode updateMode = UpdateMode.LateUpdate; // When to update the camera?
		[FormerlySerializedAs("lockCursor")] public bool LockCursor = true; // If true, the mouse will be locked to screen center and hidden

		[FormerlySerializedAs("smoothFollow")] [Header("Position")]
		public bool SmoothFollow; // If > 0, camera will smoothly interpolate towards the target
		[FormerlySerializedAs("offset")] public Vector3 Offset = new Vector3(0, 1.5f, 0.5f); // The offset from target relative to camera rotation
		[FormerlySerializedAs("followSpeed")] public float FollowSpeed = 10f; // Smooth follow speed

		[FormerlySerializedAs("rotationSensitivity")] [Header("Rotation")]
		public float RotationSensitivity = 3.5f; // The sensitivity of rotation
		[FormerlySerializedAs("yMinLimit")] public float YMinLimit = -20; // Min vertical angle
		[FormerlySerializedAs("yMaxLimit")] public float YMaxLimit = 80; // Max vertical angle
		[FormerlySerializedAs("rotateAlways")] public bool RotateAlways = true; // Always rotate to mouse?
		[FormerlySerializedAs("rotateOnLeftButton")] public bool RotateOnLeftButton; // Rotate to mouse when left button is pressed?
		[FormerlySerializedAs("rotateOnRightButton")] public bool RotateOnRightButton; // Rotate to mouse when right button is pressed?
		[FormerlySerializedAs("rotateOnMiddleButton")] public bool RotateOnMiddleButton; // Rotate to mouse when middle button is pressed?

		[FormerlySerializedAs("distance")] [Header("Distance")]
		public float Distance = 10.0f; // The current distance to target
		[FormerlySerializedAs("minDistance")] public float MinDistance = 4; // The minimum distance to target
		[FormerlySerializedAs("maxDistance")] public float MaxDistance = 10; // The maximum distance to target
		[FormerlySerializedAs("zoomSpeed")] public float ZoomSpeed = 10f; // The speed of interpolating the distance
		[FormerlySerializedAs("zoomSensitivity")] public float ZoomSensitivity = 1f; // The sensitivity of mouse zoom

		[FormerlySerializedAs("blockingLayers")] [Header("Blocking")]
		public LayerMask BlockingLayers;
		[FormerlySerializedAs("blockingRadius")] public float BlockingRadius = 1f;
		[FormerlySerializedAs("blockingSmoothTime")] public float BlockingSmoothTime = 0.1f;
		[FormerlySerializedAs("blockingOriginOffset")] public float BlockingOriginOffset;
		[FormerlySerializedAs("blockedOffset")] [Range(0f, 1f)] public float BlockedOffset = 0.5f;

		public float X { get; private set; } // The current x rotation of the camera
		public float Y { get; private set; } // The current y rotation of the camera
		public float DistanceTarget { get; private set; } // Get/set distance

		private Vector3 _targetDistance, _position;
		private Quaternion _rotation = Quaternion.identity;
		private Vector3 _smoothPosition;
		private UnityEngine.Camera _cam;
		private bool _fixedFrame;
		private float _fixedDeltaTime;
		private Quaternion _r = Quaternion.identity;
		private Vector3 _lastUp;
		private float _blockedDistance = 10f, _blockedDistanceV;

		public void SetAngles(Quaternion rotation)
		{
			Vector3 euler = rotation.eulerAngles;
			X = euler.y;
			Y = euler.x;
		}

		public void SetAngles(float yaw, float pitch)
		{
			X = yaw;
			Y = pitch;
		}

		// Initiate, set the params to the current transformation of the camera relative to the target
		protected virtual void Awake()
		{
			Vector3 angles = transform.eulerAngles;
			X = angles.y;
			Y = angles.x;

			DistanceTarget = Distance;
			_smoothPosition = transform.position;

			_cam = GetComponent<UnityEngine.Camera>();

			_lastUp = RotationSpace != null ? RotationSpace.up : Vector3.up;
		}

		protected virtual void Update()
		{
			if (updateMode == UpdateMode.Update) UpdateTransform();
		}

		protected virtual void FixedUpdate()
		{
			_fixedFrame = true;
			_fixedDeltaTime += Time.deltaTime;
			if (updateMode == UpdateMode.FixedUpdate) UpdateTransform();
		}

		protected virtual void LateUpdate()
		{
			UpdateInput();

			if (updateMode == UpdateMode.LateUpdate) UpdateTransform();

			if (updateMode == UpdateMode.FixedLateUpdate && _fixedFrame)
			{
				UpdateTransform(_fixedDeltaTime);
				_fixedDeltaTime = 0f;
				_fixedFrame = false;
			}
		}

		// Read the user input
		public void UpdateInput()
		{
			if (!_cam.enabled) return;

			// Cursors
			Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = LockCursor ? false : true;

			// Should we rotate the camera?
			bool rotate = RotateAlways || (RotateOnLeftButton && Input.GetMouseButton(0)) || (RotateOnRightButton && Input.GetMouseButton(1)) || (RotateOnMiddleButton && Input.GetMouseButton(2));

			// delta rotation
			if (rotate)
			{
				X += Input.GetAxis("Mouse X") * RotationSensitivity;
				Y = ClampAngle(Y - Input.GetAxis("Mouse Y") * RotationSensitivity, YMinLimit, YMaxLimit);
			}

			// Distance
			DistanceTarget = Mathf.Clamp(DistanceTarget + ZoomAdd, MinDistance, MaxDistance);
		}

		// Update the camera transform
		public void UpdateTransform()
		{
			UpdateTransform(Time.deltaTime);
		}

		public void UpdateTransform(float deltaTime)
		{
			if (!_cam.enabled) return;

			// Rotation
			_rotation = Quaternion.AngleAxis(X, Vector3.up) * Quaternion.AngleAxis(Y, Vector3.right);

			if (RotationSpace != null)
			{
				_r = Quaternion.FromToRotation(_lastUp, RotationSpace.up) * _r;
				_rotation = _r * _rotation;

				_lastUp = RotationSpace.up;

			}

			if (Target != null)
			{
				// Distance
				Distance += (DistanceTarget - Distance) * ZoomSpeed * deltaTime;

				// Smooth follow
				if (!SmoothFollow) _smoothPosition = Target.position;
				else _smoothPosition = Vector3.Lerp(_smoothPosition, Target.position, deltaTime * FollowSpeed);

				// Position
				Vector3 t = _smoothPosition + _rotation * Offset;
				Vector3 f = _rotation * -Vector3.forward;

				if (BlockingLayers != -1)
				{
					RaycastHit hit;
					if (Physics.SphereCast(t - f * BlockingOriginOffset, BlockingRadius, f, out hit, BlockingOriginOffset + DistanceTarget - BlockingRadius, BlockingLayers))
					{
						_blockedDistance = Mathf.SmoothDamp(_blockedDistance, hit.distance + BlockingRadius * (1f - BlockedOffset) - BlockingOriginOffset, ref _blockedDistanceV, BlockingSmoothTime);
					}
					else _blockedDistance = DistanceTarget;

					Distance = Mathf.Min(Distance, _blockedDistance);
				}

				_position = t + f * Distance;

				// Translating the camera
				transform.position = _position;
			}

			transform.rotation = _rotation;
		}

		// Zoom input
		private float ZoomAdd
		{
			get
			{
				float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
				if (scrollAxis > 0) return -ZoomSensitivity;
				if (scrollAxis < 0) return ZoomSensitivity;
				return 0;
			}
		}

		// Clamping Euler angles
		private float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360) angle += 360;
			if (angle > 360) angle -= 360;
			return Mathf.Clamp(angle, min, max);
		}

	}
}


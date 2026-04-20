using UnityEngine;
using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;

namespace _Project.Scripts.Gameplay
{
    /// <summary>
    /// Handles player movement, input processing, and boundary checking.
    /// Follows Single Responsibility Principle by managing only movement-related logic.
    /// </summary>
    public class PlayerMovementController : MonoBehaviour
    {
        [Inject] private InputReader _inputReader;

        [SerializeField] private Rigidbody _hipsRigidbody;
        [SerializeField] private float _boundaryLimit = 40f;
        [SerializeField] private float _boundaryCorrectionSpeed = 3f;

        private Rigidbody[] _bodies;
        private bool _enabled;
        private float _movementSpeed;

        public void Initialize(Rigidbody[] bodies, float movementSpeed)
        {
            _bodies = bodies;
            _movementSpeed = movementSpeed;
            _enabled = true;
        }

        public void SetEnabled(bool enabled) => _enabled = enabled;

        public void SetMovementSpeed(float speed) => _movementSpeed = speed;

        public void Configure(Rigidbody hipsRigidbody)
        {
            if (_hipsRigidbody == null)
            {
                _hipsRigidbody = hipsRigidbody;
            }
        }

        private void Update()
        {
            if (!_enabled)
                return;

            CheckForBoundaries();

            if (_inputReader.IsPointerOverUI())
                return;

            if (!_inputReader.GetMouseButton(0))
                return;

            float xValue = _inputReader.GetAxis("Mouse X");

            foreach (Rigidbody rb in _bodies)
            {
                if (rb != null)
                {
                    rb.linearVelocity += new Vector3(xValue, 0, 0) * (Time.deltaTime * _movementSpeed);
                }
            }
        }

        private void CheckForBoundaries()
        {
            if (_hipsRigidbody == null)
                return;

            float xPos = _hipsRigidbody.transform.position.x;

            if (xPos >= -_boundaryLimit && xPos <= _boundaryLimit)
                return;

            float newX = Mathf.Sign(xPos) * -_boundaryCorrectionSpeed;

            _hipsRigidbody.linearVelocity = new Vector3(newX, _hipsRigidbody.linearVelocity.y, _hipsRigidbody.linearVelocity.z);
        }
    }
}

using UnityEngine;
using System.Collections;

namespace _Project.Scripts.Gameplay
{
    /// <summary>
    /// Main player controller that orchestrates player behavior through specialized components.
    /// Acts as a facade for movement, launch, death, and initialization logic.
    /// </summary>
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerLaunchController))]
    [RequireComponent(typeof(PlayerDeathHandler))]
    [RequireComponent(typeof(PlayerInitializer))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController _movementController;
        [SerializeField] private PlayerLaunchController _launchController;
        [SerializeField] private PlayerDeathHandler _deathHandler;
        [SerializeField] private PlayerInitializer _initializer;

        private bool _enabled;
        private Transform _launchCapsule;
        private Vector3 _launchInitialPosition;

        // Public properties for backward compatibility
        public Rigidbody SelfHips => _initializer?.HipsRigidbody;
        public TrailRenderer TrailRenderer => _initializer?.TrailRenderer;
        public Rigidbody[] Bodies => _initializer?.Bodies;
        public Animator Animator => _initializer?.Animator;

        public bool IsPassed { get; set; }
        public bool IsTarget { get; set; }
        public bool IsDie { get; private set; }

        private void Awake() => CacheComponentReferences();

        private void OnValidate() => CacheComponentReferences();

        private void Start()
        {
            _movementController?.Configure(SelfHips);

            InitializeMovementController();
            InitializeLaunchController();
            InitializeDeathHandler();
        }

        private void InitializeMovementController()
        {
            if (_movementController != null && _initializer != null)
                _movementController.Initialize(Bodies, _initializer.GetMovementSpeed());
        }

        private void InitializeLaunchController()
        {
            if (_launchController != null && _initializer != null)
                _launchController.Initialize(
                    Bodies,
                    _launchCapsule,
                    _launchInitialPosition
                );
        }

        private void InitializeDeathHandler()
        {
            if (_deathHandler != null && _initializer != null)
                _deathHandler.Initialize(SelfHips, this);
        }

        public void Initialize(float maxLaunchSpeed, float movementSpeed)
        {
            _enabled = true;

            if (_movementController != null)
                _movementController.SetMovementSpeed(movementSpeed);

            if (_launchController != null)
                _launchController.UpdateMaxLaunchSpeed(maxLaunchSpeed);
        }

        public void Disable()
        {
            _enabled = false;
            if (_movementController != null)
            {
                _movementController.SetEnabled(false);
            }
        }

        public void SetInitial(FixedJoint joint, Transform capsule)
        {
            _launchCapsule = capsule;
            _launchInitialPosition = capsule != null ? capsule.position : Vector3.zero;

            if (_launchController != null)
            {
                _launchController.Initialize(
                    Bodies,
                    _launchCapsule,
                    _launchInitialPosition
                );
            }
        }

        public void Initialize()
        {
            if (_initializer != null)
                Initialize(_initializer.GetMaxLaunchSpeed(), _initializer.GetMovementSpeed());
        }

        public void CheckForHeight()
        {
            if (_deathHandler != null)
            {
                _deathHandler.CheckForDeath();
                return;
            }

            Rigidbody hips = SelfHips;
            if (hips != null && hips.transform.position.y < -5f)
            {
                Die();
            }
        }

        private void Update()
        {
            if (!_enabled)
                return;

            if (_deathHandler != null)
            {
                _deathHandler.CheckForDeath();
            }
        }

        public IEnumerator ApplyLaunchForce(float factor)
        {
            if (_launchController != null)
            {
                return _launchController.ApplyLaunchForce(factor);
            }

            return null;
        }

        public void Die()
        {
            if (_deathHandler != null)
            {
                _deathHandler.Die();
            }
            else
            {
                MarkAsDead();                
            }
        }

        public void SetupInitialState()
        {
            if (_initializer != null)
            {
                _initializer.SetupInitialState();
            }
        }
        
        public void MarkAsDead()
        {
            IsDie = true;
            IsTarget = false;
        }

        private void CacheComponentReferences()
        {
            _movementController ??= GetComponent<PlayerMovementController>();
            _launchController ??= GetComponent<PlayerLaunchController>();
            _deathHandler ??= GetComponent<PlayerDeathHandler>();
            _initializer ??= GetComponent<PlayerInitializer>();
        }
    }
}

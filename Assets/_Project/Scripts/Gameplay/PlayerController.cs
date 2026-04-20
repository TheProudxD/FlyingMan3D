using UnityEngine;
using System.Collections;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.UI;
using Reflex.Attributes;

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
        [Inject] private GameFactory _gameFactory;
        [Inject] private UIFactory _uiFactory;
        [Inject] private FxFactory _fxFactory;
        [Inject] private PlayerFactory _playerFactory;
        [Inject] private AudioService _audioService;
        [Inject] private IPersistentProgressService _persistentProgressService;

        private PlayerMovementController _movementController;
        private PlayerLaunchController _launchController;
        private PlayerDeathHandler _deathHandler;
        private PlayerInitializer _initializer;

        private bool _enabled;
        private float _maxLaunchSpeed;
        private float _movementSpeed;
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

        private void Awake()
        {
            _movementController = GetComponent<PlayerMovementController>();
            _launchController = GetComponent<PlayerLaunchController>();
            _deathHandler = GetComponent<PlayerDeathHandler>();
            _initializer = GetComponent<PlayerInitializer>();
        }

        private void Start()
        {
            if (_initializer != null)
            {
                _initializer.Initialize(_gameFactory, _uiFactory, _playerFactory, this);
            }

            _movementController?.Configure(SelfHips);

            // Initialize specialized controllers
            InitializeMovementController();
            InitializeLaunchController();
            InitializeDeathHandler();
        }

        private void InitializeMovementController()
        {
            if (_movementController != null && _initializer != null)
            {
                _movementController.Initialize(Bodies, GetMovementSpeed());
            }
        }

        private void InitializeLaunchController()
        {
            if (_launchController != null && _initializer != null)
            {
                if (_launchCapsule == null)
                    return;

                Hud hud = _uiFactory.GetHUD();
                Spawner spawner = _gameFactory.GetSpawner();

                _launchController.Initialize(
                    Bodies,
                    _launchCapsule,
                    _launchInitialPosition,
                    hud,
                    _audioService,
                    spawner,
                    _maxLaunchSpeed
                );
            }
        }

        private void InitializeDeathHandler()
        {
            if (_deathHandler != null && _initializer != null)
            {
                _deathHandler.Initialize(
                    SelfHips,
                    _fxFactory,
                    _playerFactory,
                    this
                );
            }
        }

        public void Initialize(float maxLaunchSpeed, float movementSpeed)
        {
            _enabled = true;
            _maxLaunchSpeed = maxLaunchSpeed;
            _movementSpeed = movementSpeed;

            if (_movementController != null)
            {
                _movementController.SetMovementSpeed(_movementSpeed);
            }

            if (_launchController != null)
            {
                _launchController.UpdateMaxLaunchSpeed(_maxLaunchSpeed);
            }
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

            // Update launch controller with capsule and initial position
            if (_launchController != null)
            {
                _launchController.Initialize(
                    Bodies,
                    _launchCapsule,
                    _launchInitialPosition,
                    _uiFactory.GetHUD(),
                    _audioService,
                    _gameFactory.GetSpawner(),
                    _maxLaunchSpeed
                );
            }
        }

        public float GetMovementSpeed() => _persistentProgressService.PowerupProgress.flyingControl;

        public void Initialize()
        {
            float maxLaunchSpeed = _gameFactory.GetCurrentLevel().MaxLaunchSpeed;
            float movementSpeed = GetMovementSpeed();
            Initialize(maxLaunchSpeed, movementSpeed);
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

            // Delegate death checking to death handler
            if (_deathHandler != null)
            {
                _deathHandler.CheckForDeath();
            }
        }

        public IEnumerator ApplyLaunchForce(float factor)
        {
            // Delegate to launch controller
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
                // Fallback for backward compatibility
                IsDie = true;
                IsTarget = false;
                _playerFactory.RemovePlayer(this);
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
    }
}

using UnityEngine;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using Reflex.Attributes;

namespace _Project.Scripts.Gameplay
{
    /// <summary>
    /// Handles player initialization, component setup, and dependency injection.
    /// Manages the initial state and connections between player components.
    /// </summary>
    public class PlayerInitializer : MonoBehaviour
    {
        [Inject] private GameFactory _gameFactory;
        [Inject] private IPersistentProgressService _persistentProgressService;

        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Rigidbody _hipsRigidbody;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Rigidbody[] _bodies;
        [SerializeField] private Animator _animator;

        private void Awake() => CacheReferences();

        private void OnValidate() => CacheReferences();

        public void SetupInitialState()
        {
            if (_playerController == null)
                return;

            _playerController.Initialize(GetMaxLaunchSpeed(), GetMovementSpeed());
        }

        public void SetInitialJoint(FixedJoint joint, Transform capsule)
        {
            if (_playerController != null)
                _playerController.SetInitial(joint, capsule);
        }

        public float GetMovementSpeed() => _persistentProgressService.PowerupProgress.flyingControl;

        public float GetMaxLaunchSpeed() => _gameFactory.GetCurrentLevel().MaxLaunchSpeed;

        // Public accessors for other components
        public Rigidbody HipsRigidbody
        {
            get
            {
                if (_hipsRigidbody == null)
                {
                    CacheReferences();

                    if (_hipsRigidbody == null)
                        _hipsRigidbody = GetComponentInChildren<Rigidbody>(true);
                }

                return _hipsRigidbody;
            }
        }
        public TrailRenderer TrailRenderer => _trailRenderer;
        public Rigidbody[] Bodies => _bodies;
        public Animator Animator => _animator;

        private void CacheReferences()
        {
            _playerController ??= GetComponent<PlayerController>();
            _animator ??= GetComponent<Animator>();

            if (_bodies == null || _bodies.Length == 0)
                _bodies = GetComponentsInChildren<Rigidbody>(true);

            if (_hipsRigidbody == null)
                _hipsRigidbody = GetComponentInChildren<Rigidbody>(true);
        }
    }
}

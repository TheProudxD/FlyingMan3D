using UnityEngine;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI;

namespace _Project.Scripts.Gameplay
{
    /// <summary>
    /// Handles player initialization, component setup, and dependency injection.
    /// Manages the initial state and connections between player components.
    /// </summary>
    public class PlayerInitializer : MonoBehaviour
    {
        [SerializeField] private Rigidbody _hipsRigidbody;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Rigidbody[] _bodies;
        [SerializeField] private Animator _animator;

        private GameFactory _gameFactory;
        private UIFactory _uiFactory;
        private PlayerFactory _playerFactory;
        private PlayerController _playerController;
        private bool _isInitialized;

        private void Awake() => CacheReferences();

        private void OnValidate() => CacheReferences();

        public void Initialize(
            GameFactory gameFactory,
            UIFactory uiFactory,
            PlayerFactory playerFactory,
            PlayerController playerController)
        {
            if (_isInitialized)
                return;

            _gameFactory = gameFactory;
            _uiFactory = uiFactory;
            _playerFactory = playerFactory;
            _playerController = playerController;

            _isInitialized = true;
        }

        public void SetupInitialState()
        {
            float maxLaunchSpeed = _gameFactory.GetCurrentLevel().MaxLaunchSpeed;
            float movementSpeed = _playerController.GetMovementSpeed();

            _playerController.Initialize(maxLaunchSpeed, movementSpeed);
        }

        public void SetInitialJoint(FixedJoint joint, Transform capsule)
        {
            _playerController.SetInitial(joint, capsule);
        }

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
            _animator ??= GetComponent<Animator>();

            if (_bodies == null || _bodies.Length == 0)
                _bodies = GetComponentsInChildren<Rigidbody>(true);

            if (_hipsRigidbody == null)
                _hipsRigidbody = GetComponentInChildren<Rigidbody>(true);
        }
    }
}

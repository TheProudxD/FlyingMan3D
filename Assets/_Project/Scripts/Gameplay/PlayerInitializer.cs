using UnityEngine;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using Reflex.Attributes;

namespace _Project.Scripts.Gameplay
{
    public class PlayerInitializer : MonoBehaviour
    {
        [Inject] private GameFactory _gameFactory;
        [Inject] private IPersistentProgressService _persistentProgressService;

        [SerializeField] private PlayerMovementController _movementController;
        [SerializeField] private PlayerLaunchController _launchController;
        [SerializeField] private Rigidbody _hipsRigidbody;
        [SerializeField] private Rigidbody[] _bodies;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Animator _animator;

        private void Awake() => CacheReferences();

        private void OnValidate() => CacheReferences();

        public void InitializeStructuralComponents()
        {
            _movementController?.Configure(_hipsRigidbody);
            _movementController?.Initialize(_bodies, GetMovementSpeed());
            _launchController?.ConfigureBodies(_bodies);
        }

        public void ApplyRuntimeSettings()
        {
            _movementController?.SetMovementSpeed(GetMovementSpeed());

            Level currentLevel = _gameFactory?.GetCurrentLevel();
            if (currentLevel != null)
                _launchController?.SetMaxLaunchSpeed(currentLevel.MaxLaunchSpeed);
        }

        public float GetMovementSpeed() => _persistentProgressService.PowerupProgress.flyingControl;
        
        public Rigidbody SelfHips => _hipsRigidbody;
        public TrailRenderer TrailRenderer => _trailRenderer;
        public Rigidbody[] Bodies => _bodies;
        public Animator Animator => _animator;
        
        private void CacheReferences()
        {
            _movementController ??= GetComponent<PlayerMovementController>();
            _launchController ??= GetComponent<PlayerLaunchController>();
            _animator ??= GetComponent<Animator>();
            _trailRenderer ??= GetComponentInChildren<TrailRenderer>(true);
            
            if (_hipsRigidbody == null)
                _hipsRigidbody = GetComponentInChildren<Rigidbody>(true);
            
            if (_bodies == null || _bodies.Length == 0)
                _bodies = GetComponentsInChildren<Rigidbody>(true);
        }
    }
}

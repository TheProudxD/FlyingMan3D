using System.Collections;
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
        [SerializeField] private PlayerDeathHandler _deathHandler;
        [SerializeField] private Rigidbody _hipsRigidbody;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Rigidbody[] _bodies;
        [SerializeField] private Animator _animator;

        private void Awake() => CacheReferences();

        private void OnValidate() => CacheReferences();

        public void InitializeStructuralComponents()
        {
            _movementController?.Configure(_hipsRigidbody);
            _movementController?.Initialize(_bodies, GetMovementSpeed());
            _launchController?.ConfigureBodies(_bodies);
        }

        public void SetLaunchAnchor(Transform launchCapsule)
        {
            _launchController?.SetLaunchAnchor(launchCapsule);
        }

        public void ApplyRuntimeSettings()
        {
            _movementController?.SetMovementSpeed(GetMovementSpeed());

            Level currentLevel = _gameFactory?.GetCurrentLevel();
            if (currentLevel != null)
                _launchController?.SetMaxLaunchSpeed(currentLevel.MaxLaunchSpeed);
        }

        public void DisableMovement() => _movementController?.SetEnabled(false);

        public void CheckForDeath() => _deathHandler?.CheckForDeath();

        public IEnumerator ApplyLaunchForce(float factor) =>
            _launchController?.ApplyLaunchForce(factor);

        public void Die() => _deathHandler?.Die();

        public float GetMovementSpeed() => _persistentProgressService.PowerupProgress.flyingControl;

        public Rigidbody HipsRigidbody => _hipsRigidbody;
        public TrailRenderer TrailRenderer => _trailRenderer;
        public Rigidbody[] Bodies => _bodies;
        public Animator Animator => _animator;

        private void CacheReferences()
        {
            _movementController ??= GetComponent<PlayerMovementController>();
            _launchController ??= GetComponent<PlayerLaunchController>();
            _deathHandler ??= GetComponent<PlayerDeathHandler>();
            _animator ??= GetComponent<Animator>();
            _trailRenderer ??= GetComponentInChildren<TrailRenderer>(true);

            if (_bodies == null || _bodies.Length == 0)
                _bodies = GetComponentsInChildren<Rigidbody>(true);

            if (_hipsRigidbody == null)
                _hipsRigidbody = GetComponentInChildren<Rigidbody>(true);
        }
    }
}

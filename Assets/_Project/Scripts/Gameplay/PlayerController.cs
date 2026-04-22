using UnityEngine;
using System.Collections;

namespace _Project.Scripts.Gameplay
{
    [RequireComponent(typeof(PlayerMovementController))]
    [RequireComponent(typeof(PlayerLaunchController))]
    [RequireComponent(typeof(PlayerDeathHandler))]
    [RequireComponent(typeof(PlayerInitializer))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInitializer _initializer;

        private bool _enabled;
        private Transform _launchCapsule;

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
            _initializer?.InitializeStructuralComponents();
        }

        public void Disable()
        {
            _enabled = false;
            _initializer?.DisableMovement();
        }

        public void SetInitial(Transform capsule)
        {
            _launchCapsule = capsule;
            _initializer?.SetLaunchAnchor(_launchCapsule);
        }

        public void Initialize()
        {
            _enabled = true;
            _initializer?.ApplyRuntimeSettings();
        }

        public void CheckForHeight() => _initializer?.CheckForDeath();

        private void Update()
        {
            if (!_enabled)
                return;

            _initializer?.CheckForDeath();
        }

        public IEnumerator ApplyLaunchForce(float factor) => _initializer?.ApplyLaunchForce(factor);

        public void Die() => _initializer?.Die();
        
        public void MarkAsDead()
        {
            IsDie = true;
            IsTarget = false;
        }

        private void CacheComponentReferences()
        {
            _initializer ??= GetComponent<PlayerInitializer>();
        }
    }
}

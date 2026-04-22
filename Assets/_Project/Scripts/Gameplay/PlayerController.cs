using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
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
        
        // Public properties for backward compatibility
        public Rigidbody SelfHips => _initializer.SelfHips;
        public TrailRenderer TrailRenderer => _initializer.TrailRenderer;
        public Rigidbody[] Bodies => _initializer.Bodies;
        public Animator Animator => _initializer.Animator;

        public bool IsPassed { get; set; }
        public bool IsTarget { get; set; }
        public bool IsDie { get; private set; }

        private bool _enabled;
        
        private void Awake() => CacheComponentReferences();

        private void OnValidate() => CacheComponentReferences();

        private void Start()
        {
            _initializer?.InitializeStructuralComponents();
        }

        public void Disable()
        {
            _enabled = false;
            _movementController?.SetEnabled(false);
        }

        public void SetInitial(Transform capsule)
        {
            _launchController?.SetLaunchAnchor(capsule);
        }

        public void Initialize()
        {
            _enabled = true;
            _initializer?.ApplyRuntimeSettings();
        }

        private void Update()
        {
            if (!_enabled)
                return;

            CheckForHeight();
        }

        public void CheckForHeight() => _deathHandler?.CheckForDeath();

        public IEnumerator ApplyLaunchForce(float factor) =>
            _launchController?.ApplyLaunchForce(factor);

        public void Die() => _deathHandler?.Die();
        
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
        }
    }
}

using UnityEngine;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.Factories;

namespace _Project.Scripts.Gameplay
{
    /// <summary>
    /// Handles player death detection, ragdoll creation, and cleanup.
    /// Manages the death state and coordinates with player factory for removal.
    /// </summary>
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField] private float _dieHeight = -5f;

        private FxFactory _fxFactory;
        private PlayerFactory _playerFactory;
        private PlayerController _playerController;

        private Rigidbody _hipsRigidbody;
        private bool _isDead;

        public void Initialize(
            Rigidbody hipsRigidbody,
            FxFactory fxFactory,
            PlayerFactory playerFactory,
            PlayerController playerController)
        {
            _hipsRigidbody = hipsRigidbody;
            _fxFactory = fxFactory;
            _playerFactory = playerFactory;
            _playerController = playerController;
        }

        public void CheckForDeath()
        {
            if (_isDead || _hipsRigidbody == null)
                return;

            float yPos = _hipsRigidbody.transform.position.y;

            if (yPos < _dieHeight)
            {
                Die();
            }
        }

        public void Die()
        {
            if (_isDead)
                return;

            DieAsync().Forget();
        }

        private async UniTask DieAsync()
        {
            _isDead = true;
            _playerController.MarkAsDead();

            // Create ragdoll effect if object is still active
            if (transform != null && gameObject != null && gameObject.activeInHierarchy)
            {
                await _fxFactory.CreatePlayerRagdoll(transform.position, Quaternion.identity);
            }

            // Remove player from factory
            _playerFactory.RemovePlayer(_playerController);
        }
    }
}

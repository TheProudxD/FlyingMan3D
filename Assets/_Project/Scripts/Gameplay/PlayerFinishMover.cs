using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Gameplay
{
    public class PlayerFinishMover : MonoBehaviour
    {
        [Inject] private GameFactory _gameFactory;
        [Inject] private PlayerFinishCombatService _playerFinishCombatService;
        [Inject] private IPersistentProgressService _persistentProgressService;

        private static readonly int IsGround = Animator.StringToHash("IsGround");

        [SerializeField] private PlayerController _playerController;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Canvas _healthCanvas;
        [SerializeField] private LayerMask _groundLayer;

        private float _raycastDistance = 100;
        private readonly RaycastHit[] _raycastHits = new RaycastHit[2];

        private float _moveSpeed;
        private float _stopDistance;
        private GameObject _target;
        private Vector3 _moveDistance;
        private bool _canSmoke = true;
        private bool _canMove;
        private float _rotationSpeed = 100;
        private int _health;
        private int _damage = 1;

        public int Health
        {
            get => _health;
            set
            {
                _health = value;

                if (_healthText != null)
                    _healthText.SetText(_health.ToString());
            }
        }

        public void Initialize()
        {
            enabled = true;
            _target = null;
            _canSmoke = true;

            if (_gameFactory.GetCurrentLevel() == null || _persistentProgressService?.PowerupProgress == null)
                return;

            _stopDistance = _gameFactory.GetCurrentLevel().StopDistance;
            _moveSpeed = _persistentProgressService.PowerupProgress.movingSpeed;
            Health = Random.Range(1, _persistentProgressService.PowerupProgress.health + 1);

            if (_healthCanvas != null)
                _healthCanvas.gameObject.SetActive(true);
        }

        private void Start()
        {
            if (gameObject.CompareTag("Enemy"))
            {
                _canMove = true;
            }
        }

        private void Update()
        {
            _playerController.CheckForHeight();

            if (!_canMove)
                return;

            if (!HasValidTarget())
                _target = _playerFinishCombatService.GetNearestTarget(transform.position);

            if (HasValidTarget())
            {
                _moveDistance = _target.transform.position - transform.position;
                _moveDistance.y = 0f;

                if (_moveDistance.magnitude <= _stopDistance)
                    return;

                Quaternion targetRotation = Quaternion.LookRotation(_moveDistance);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * (_moveSpeed * Time.deltaTime));
            }
        }

        private bool HasValidTarget()
        {
            if (_target == null || !_target.activeInHierarchy)
                return false;

            if (_target.TryGetComponent(out EnemyBase enemy))
                return !enemy.IsDie;

            return true;
        }

        public bool IsGrounded()
        {
            int hitsCount = Physics.RaycastNonAlloc(
                _playerController.SelfHips.position,
                Vector3.down,
                _raycastHits,
                _raycastDistance,
                _groundLayer
            );

            return hitsCount > 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            FightAsync(collision).Forget();
        }

        private void OnCollisionStay(Collision other)
        {
            FightAsync(other).Forget();
        }

        private async UniTask FightAsync(Collision collision)
        {
            PlayerFinishCollisionResult result = await _playerFinishCombatService.ResolveCollision(
                _playerController,
                transform,
                collision,
                Health,
                _damage,
                _canSmoke
            );

            Health = result.Health;
            _canSmoke = result.CanSmoke;

            if (!result.ShouldEnableMovement)
                return;

            _playerController.Animator.SetBool(IsGround, true);
            _canMove = true;
        }
    }
}

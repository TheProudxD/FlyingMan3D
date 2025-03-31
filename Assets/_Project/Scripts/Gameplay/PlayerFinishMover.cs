using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

public class PlayerFinishMover : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    [Inject] private AssetProvider _assetProvider;

    private static readonly int IsGround = Animator.StringToHash("IsGround");
    [SerializeField] private PlayerController _playerController;

    private float _moveSpeed;
    private float _stopDistance;
    private GameObject _target;
    private Vector3 _moveDistance;
    private bool _canSmoke = true;
    private float _rotationSpeed = 100;
    public bool CanMove { get; private set; }

    public void Initialize()
    {
        _moveSpeed = _gameFactory.GetCurrentLevel().MovementSpeed;
        _stopDistance = _gameFactory.GetCurrentLevel().StopDistance;
    }

    private void Start()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            CanMove = true;
        }
    }

    private void Update()
    {
        if (!CanMove)
            return;

        if (_target == null)
        {
            _target = NearestTarget();
        }
        else
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

    private GameObject NearestTarget()
    {
        float minDistance = float.MaxValue;
        int index = 0;

        for (int i = 1; i < _gameFactory.Enemies.Count; i++)
        {
            float distance = Distance(_gameFactory.Enemies[i].transform.position, transform.position);

            if (minDistance <= distance)
                continue;

            minDistance = distance;
            index = i;
        }

        _target = _gameFactory.Enemies.Count > 0 ? _gameFactory.Enemies[index].gameObject : null;

        return _target;
    }

    private float Distance(Vector3 v1, Vector3 v2) => (v1 - v2).magnitude;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.TryGetComponent(out Enemy enemy) && !enemy.IsDie && !_playerController.IsDie)
        {
            if (_canSmoke)
            {
                _canSmoke = false;
                _assetProvider.GetSmoke(new Vector3(0f, 2f, transform.position.z), Quaternion.Euler(-90f, 0f, 0f));
            }

            enemy.Die();
            _playerController.Die();
        }

        if (gameObject.transform.root.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("Platform") == false)
            return;

        _playerController.Animator.SetBool(IsGround, true);
        CanMove = true;
    }
}
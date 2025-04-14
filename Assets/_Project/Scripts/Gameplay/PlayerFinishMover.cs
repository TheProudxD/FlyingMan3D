using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using YG;

public class PlayerFinishMover : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    [Inject] private AudioService _audioService;

    private static readonly int IsGround = Animator.StringToHash("IsGround");
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Canvas _healthCanvas;

    private float _moveSpeed;
    private float _stopDistance;
    private GameObject _target;
    private Vector3 _moveDistance;
    private bool _canSmoke = true;
    private bool _canMove;
    private float _rotationSpeed = 100;
    private int _health;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            _healthText.SetText(_health.ToString());
        }
    }

    public void Initialize()
    {
        enabled = true;
        _stopDistance = _gameFactory.GetCurrentLevel().StopDistance;
        _moveSpeed = YG2.saves.movingSpeed;
        Health = YG2.saves.health;
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
        if (!_canMove)
            return;

        _playerController.CheckForHeight();

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
            _audioService.PlayHitSound();
            if (_canSmoke)
            {
                _canSmoke = false;
                _gameFactory.GetSmoke(new Vector3(0f, 2f, transform.position.z), Quaternion.Euler(-90f, 0f, 0f));
            }

            enemy.Die();
            Health--;

            if (Health <= 0)
            {
                _audioService.PlayDieSound();
                _playerController.Die();
            }
        }

        if (gameObject.transform.root.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("Platform") == false)
            return;

        _playerController.Animator.SetBool(IsGround, true);
        _canMove = true;
    }
}
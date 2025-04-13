using System;
using System.Linq;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;

    private static readonly int CanAttack = Animator.StringToHash("CanAttack");

    public bool IsDie { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }

    private GameObject _target;
    private float _minDistance;
    private int _index;
    private Vector3 _moveDistance;
    private Collider[] _colliders;
    private bool _enabled;
    private float _stopDistance = 0.2f;
    private float _moveSpeed = 2.4f;

    public void SetAttackState()
    {
        _enabled = true;
        Animator.SetBool(CanAttack, true);
        _stopDistance = _gameFactory.GetCurrentLevel().EnemyStopDistance;
        _moveSpeed = _gameFactory.GetCurrentLevel().EnemyMoveSpeed;
    }

    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>();

        for (int i = 1; i < _colliders.Length; i++)
        {
            Destroy(_colliders[i]);
        }
    }

    private GameObject NearestTarget()
    {
        _index = 0;
        _minDistance = float.MaxValue;

        if (_gameFactory.Players == null ||
            _gameFactory.Players.Any(p => p?.Animator?.enabled == false) ||
            _gameFactory.Players.Count == 0)
        {
            return null;
        }

        for (int i = 1; i < _gameFactory.Players.Count; i++)
        {
            PlayerController player = _gameFactory.Players[i];

            if (player.IsTarget)
                continue;

            float distance = Distance(player.transform.position, transform.position);

            if (_minDistance <= distance)
                continue;

            _minDistance = distance;
            _index = i;
        }

        PlayerController p = _gameFactory.Players[_index];
        _target = p.gameObject;
        p.IsTarget = true;
        return _target;
    }

    private float Distance(Vector3 v1, Vector3 v2) => (v1 - v2).magnitude;

    private void Update()
    {
        if (_enabled == false)
            return;

        if (_target == null)
        {
            Animator.SetBool(CanAttack, false);
            _target = NearestTarget();
        }
        else
        {
            Animator.SetBool(CanAttack, true);
            _moveDistance = _target.transform.position - transform.position;
            _moveDistance.y = 0f;

            if (_moveDistance.magnitude <= _stopDistance)
                return;

            transform.position += _moveDistance.normalized * (_moveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(_moveDistance);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 120f);
        }
    }

    public void Die()
    {
        IsDie = true;
        _gameFactory.RemoveEnemy(this);
        var ragdoll = _gameFactory.GetEnemyRagdoll(transform.position, Quaternion.identity);
        ragdoll.GetComponentInChildren<Rigidbody>().AddForce(-transform.forward * 800, ForceMode.Impulse);
        Destroy(gameObject);
    }
}
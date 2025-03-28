using System;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    [Inject] private AssetProvider _assetProvider;
    
    private static readonly int CanAttack = Animator.StringToHash("CanAttack");

    public bool IsDie { get; private set; }
    public Animator Animator { get; private set; }
    
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

    private void Start()
    {
        _colliders = GetComponentsInChildren<Collider>();

        for (int i = 1; i < _colliders.Length; i++)
        {
            Destroy(_colliders[i]);
        }

        Animator = GetComponent<Animator>();
    }

    private GameObject NearestTarget()
    {
        _index = 0;
        _minDistance = float.MaxValue;
        
        if (_gameFactory.Players == null || _gameFactory.Players.Count == 0) return null;

        for (int i = 1; i < _gameFactory.Players.Count; i++)
        {
            float distance = Distance(_gameFactory.Players[i].transform.position, transform.position);

            if (_minDistance <= distance)
                continue;

            _minDistance = distance;
            _index = i;
        }

        _target = _gameFactory.Players.Count > 0 ? _gameFactory.Players[_index].gameObject : null;

        return _target;
    }

    private float Distance(Vector3 v1, Vector3 v2) => (v1 - v2).magnitude;

    private void Update()
    {
        if (_enabled == false)
            return;
        
        if (_target == null)
        {
            _target = NearestTarget();
        }
        else
        {
            try
            {
                _moveDistance = _target.transform.position - transform.position;
                _moveDistance.y = 0f;

                if (_moveDistance.magnitude <= _stopDistance)
                    return;

                transform.position += _moveDistance.normalized * (_moveSpeed * Time.deltaTime);
                Quaternion targetRotation = Quaternion.LookRotation(_moveDistance);

                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 120f);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    public void Die()
    {
        IsDie = true;
        _gameFactory.Enemies.Remove(this);
        _assetProvider.GetEnemyRagdoll(transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
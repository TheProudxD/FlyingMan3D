using System;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

public class PlayerMoveToFinish : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    [Inject] private AssetProvider _assetProvider;

    private static readonly int IsGround = Animator.StringToHash("IsGround");

    private float _moveSpeed = 2.4f;
    private float _stopDistance = 0.2f;
    private GameObject _target;
    private Vector3 _moveDistance;
    private bool _canMove;
    private bool _isDie;
    private bool _canSmoke = true;

    public void Initialize() { }

    private void Start()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            _canMove = true;
        }
    }

    private void Update()
    {
        if (!_canMove) return;

        if (_target == null)
        {
            _target = NearestTarget();
        }
        else
        {
            _moveDistance = _target.transform.position - transform.position;
            _moveDistance.y = 0f;

            if (_moveDistance.magnitude <= _stopDistance) return;

            // transform.position += _moveDistance.normalized * (_moveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(_moveDistance);
            float rotationSpeed = 120;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * (_moveSpeed * Time.deltaTime));
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 120f);
        }
    }

    private GameObject NearestTarget()
    {
        float minDistance = float.MaxValue;
        int index = 0;

        for (int i = 1; i < _gameFactory.Enemies.Count; i++)
        {
            if (!(minDistance > Distance(_gameFactory.Enemies[i].transform.position, transform.position))) continue;

            minDistance = Distance(_gameFactory.Enemies[i].transform.position, transform.position);
            index = i;
        }

        _target = _gameFactory.Enemies.Count > 0 ? _gameFactory.Enemies[index].gameObject : null;

        return _target;
    }

    private float Distance(Vector3 v1, Vector3 v2) => (v1 - v2).magnitude;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.root.TryGetComponent(out EnemyFinish finish) && !finish.IsDie && !_isDie)
        {
            if (_canSmoke)
            {
                _canSmoke = false;

                _assetProvider.GetSmoke(new Vector3(0f, 2f, transform.position.z), Quaternion.Euler(-90f, 0f, 0f));
            }

            collision.gameObject.transform.root.GetComponent<EnemyFinish>().IsDie = true;
            _gameFactory.Enemies.Remove(collision.gameObject.transform.root.gameObject.GetComponent<EnemyFinish>());
            _assetProvider.GetEnemyRagdoll(transform.position, Quaternion.identity);
            Destroy(collision.gameObject.transform.root.gameObject);

            _isDie = true;
            _gameFactory.Players.Remove(gameObject.GetComponent<PlayerController>());
            _assetProvider.GetPlayerRagdoll(transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        if (gameObject.transform.root.gameObject.CompareTag("Enemy") || !collision.gameObject.CompareTag("Platform"))
            return;

        GetComponent<Animator>().SetBool(IsGround, true);
        _canMove = true;
    }
}
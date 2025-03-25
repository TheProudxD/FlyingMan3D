using UnityEngine;

public class EnemyFinish : MonoBehaviour
{
    public bool IsDie { get; set; }

    private GameObject _target;
    private float _minDistance;
    private int _index;
    private Vector3 _moveDistance;
    private Collider[] _colliders;
    private readonly float _stopDistance = 0.2f;
    private readonly float _moveSpeed = 2.4f;

    private void Start()
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
        if (PlayerController.players == null || PlayerController.players.Count == 0) return null;

        for (int i = 1; i < PlayerController.players.Count; i++)
        {
            float distance = Distance(PlayerController.players[i].transform.position, transform.position);

            if (_minDistance <= distance)
                continue;

            _minDistance = distance;
            _index = i;
        }

        _target = PlayerController.players.Count > 0 ? PlayerController.players[_index].gameObject : null;

        return _target;
    }

    private float Distance(Vector3 v1, Vector3 v2) => (v1 - v2).magnitude;

    private void Update()
    {
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
            catch
            {
                // ignored
            }
        }
    }
}
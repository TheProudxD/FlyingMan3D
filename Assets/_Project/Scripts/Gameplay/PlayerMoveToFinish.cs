using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMoveToFinish : MonoBehaviour
{
    [FormerlySerializedAs("SmokePrefab")] [SerializeField] private GameObject _smokePrefab;
    [FormerlySerializedAs("RagdollPrefab")] [SerializeField] private GameObject _ragdollPrefab;
    [FormerlySerializedAs("EnemyRagdollPrefab")] [SerializeField] private GameObject _enemyRagdollPrefab;

    private Vector3 _moveDistance;
    private float _moveSpeed = 2.4f;
    private float _stopDistance = 0.2f;
    private bool _canMove = false;
    private bool _isDie = false;
    private GameObject _target;

    private void Start()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            _canMove = true;
        }
    }

    private void Update()
    {
        if (_canMove)
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

                    if (_moveDistance.magnitude > _stopDistance)
                    {
                        transform.position += _moveDistance.normalized * _moveSpeed * Time.deltaTime;
                        Quaternion targetRotation = Quaternion.LookRotation(_moveDistance);

                        transform.rotation =
                            Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 120f);
                    }
                }
                catch { }
            }
        }
    }

    private GameObject NearestTarget()
    {
        float minDistance = float.MaxValue;
        int index = 0;

        for (int i = 1; i < Spawner.Enemies.Count; i++)
        {
            if (minDistance > Distance(Spawner.Enemies[i].transform.position, transform.position))
            {
                minDistance = Distance(Spawner.Enemies[i].transform.position, transform.position);
                index = i;
            }
        }

        if (Spawner.Enemies.Count > 0)
        {
            _target = Spawner.Enemies[index].gameObject;
        }
        else
        {
            _target = null;
        }

        return _target;
    }

    private float Distance(Vector3 v1, Vector3 v2)
    {
        return (v1 - v2).magnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.root.gameObject.tag == "Enemy")
        {
            if (!collision.gameObject.transform.root.GetComponent<EnemyFinish>().IsDie && !_isDie)
            {
                if (GameManager.Instance.CanSmoke)
                {
                    GameManager.Instance.CanSmoke = false;
                    Instantiate(_smokePrefab, new Vector3(0f, 2f, transform.position.z), Quaternion.Euler(-90f, 0f, 0f));
                }

                collision.gameObject.transform.root.GetComponent<EnemyFinish>().IsDie = true;
                Spawner.Enemies.Remove(collision.gameObject.transform.root.gameObject);
                GameObject enemy = Instantiate(_enemyRagdollPrefab, transform.position, Quaternion.identity);
                enemy.layer = 8;
                Destroy(collision.gameObject.transform.root.gameObject);

                _isDie = true;
                PlayerController.players.Remove(gameObject.GetComponent<PlayerController>());
                GameObject self = Instantiate(_ragdollPrefab, transform.position, Quaternion.identity);
                self.layer = 8;
                Destroy(gameObject);
            }
        }

        if (gameObject.transform.root.gameObject.tag != "Enemy" && collision.gameObject.tag == "Platform")
        {
            GetComponent<Animator>().SetBool("IsGround", true);
            _canMove = true;
        }
    }
}
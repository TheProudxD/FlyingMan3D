using UnityEngine;

public class LargeEnemy : EnemyBase
{
    private static readonly int CanAttack = Animator.StringToHash("CanAttack");

    private Vector3 _moveDistance;
    private Collider[] _colliders;

    protected override float StopDistance => 0.35f;
    protected override float MoveSpeed => 0.95f;
    protected override int MaxHealth => 3;

    public override void Initialize()
    {
        Enabled = true;
        Animator.SetBool(CanAttack, true);
        Health = MaxHealth;
    }

    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>();

        for (int i = 1; i < _colliders.Length; i++)
        {
            Destroy(_colliders[i]);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Target == null)
        {
            Animator.SetBool(CanAttack, false);
            Target = NearestTarget();
        }
        else
        {
            Animator.SetBool(CanAttack, true);
            _moveDistance = Target.transform.position - transform.position;
            _moveDistance.y = 0f;

            if (_moveDistance.magnitude <= StopDistance)
                return;

            transform.position += _moveDistance.normalized * (MoveSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(_moveDistance);

            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 120f);
        }
    }
}
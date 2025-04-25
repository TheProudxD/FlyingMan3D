using System;
using System.Linq;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Inject] protected GameFactory GameFactory;

    [field: SerializeField] public Animator Animator { get; private set; }

    public bool IsDie { get; protected set; }

    protected abstract float StopDistance { get; }
    protected abstract float MoveSpeed { get; }
    protected abstract int MaxHealth { get; }

    protected int Health { get; set; }
    protected bool Enabled;
    protected GameObject Target;

    private float _minDistance;
    private int _index;

    public abstract void Initialize();

    public void TakeDamage(int damage)
    {
        Health = Math.Clamp(Health - damage, 0, int.MaxValue);

        if (Health <= 0)
        {
            Die();
        }
    }

    protected GameObject NearestTarget()
    {
        _index = 0;
        _minDistance = float.MaxValue;

        if (GameFactory.Players == null ||
            GameFactory.Players.Any(p => p?.Animator?.enabled == false) ||
            GameFactory.Players.Count == 0)
        {
            return null;
        }

        for (int i = 1; i < GameFactory.Players.Count; i++)
        {
            PlayerController player = GameFactory.Players[i];

            if (player.IsTarget)
                continue;

            float distance = Distance(player.transform.position, transform.position);

            if (_minDistance <= distance)
                continue;

            _minDistance = distance;
            _index = i;
        }

        PlayerController p = GameFactory.Players[_index];
        Target = p.gameObject;
        p.IsTarget = true;
        return Target;
    }

    protected void Die()
    {
        IsDie = true;
        GameFactory.RemoveEnemy(this);
        var ragdoll = GameFactory.GetEnemyRagdoll(transform.position, Quaternion.identity);
        ragdoll.GetComponentInChildren<Rigidbody>().AddForce(-transform.forward * 300, ForceMode.Impulse);
        Destroy(gameObject);
    }

    protected float Distance(Vector3 v1, Vector3 v2) => (v1 - v2).magnitude;

    protected virtual void Update()
    {
        if (Enabled == false)
            return;

        CheckForHeight();
        CheckForBoundaries();
    }

    private void CheckForBoundaries()
    {
        float xPos = transform.position.x;
        const float DELTA = 25f;

        if (xPos is < DELTA and > -DELTA)
            return;

        Die();
    }

    private void CheckForHeight()
    {
        float yPos = transform.position.y;

        if (yPos < -5)
            Die();
    }
}
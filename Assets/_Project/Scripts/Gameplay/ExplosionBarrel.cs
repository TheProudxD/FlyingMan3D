using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using LitMotion;
using Reflex.Attributes;
using UnityEngine;

public class ExplosionBarrel : MonoBehaviour
{
    [Inject] private AnimationService _animationService;
    [Inject] private AudioService _audioService;

    [SerializeField] private float _force;
    [SerializeField] private float _radiusDamage;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private ParticleSystem _splashParticle;
    private readonly Collider[] _results = new Collider[30];

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerFinishMover _) &&
            !other.gameObject.TryGetComponent(out Enemy _) &&
            !other.gameObject.TryGetComponent(out PlayerController _))
            return;

        _animationService.ShakingScale(transform, 0.95f, 1.5f, 0.3f, 2, Ease.InBounce, () =>
        {
            PushObjects();
            _audioService?.PlayHitSound();
            _splashParticle?.Play();

            _animationService.Scale(transform, Vector3.one, Vector3.zero, 0.5f, 1, Ease.OutBounce,
                callback: () => Destroy(gameObject));
        });
    }

    private void PushObjects()
    {
        Physics.OverlapSphereNonAlloc(transform.position, _radiusDamage, _results, _layerMask);

        foreach (Collider col in _results)
        {
            if (col == null)
                continue;

            if (!col.TryGetComponent(out Rigidbody rg))
                continue;

            Vector3 direction = (col.transform.position - transform.position).normalized;
            rg.AddForce(direction * _force, ForceMode.Impulse);
        }
    }
}
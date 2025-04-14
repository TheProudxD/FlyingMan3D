using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public abstract class RingBase : MonoBehaviour
{
    [Inject] protected GameFactory GameFactory;
    [Inject] protected AudioService AudioService;

    protected abstract string Key { get; }
    public int Effect { get; set; }
    public Vector3 MovementAxis { get; set; }
    public float Speed { get; set; }

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _movingToTarget = true;
    
    private void Start()
    {
        _startPosition = transform.position;
        _targetPosition = _startPosition + MovementAxis;
        
        var text = GetComponentInChildren<TMP_Text>();
        text.SetText(Key + Effect);
    }

    private void Update()
    {
        if (MovementAxis == Vector3.zero)
            return;

        Vector3 currentTarget = _movingToTarget ? _targetPosition : _startPosition;

        transform.position = Vector3.MoveTowards(transform.position, currentTarget, Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            _movingToTarget = !_movingToTarget;
        }
    }
}
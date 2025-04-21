using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Tools;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class Indicator : MonoBehaviour
{
    [Inject] private StateMachine _stateMachine;
    [Inject] private GameFactory _gameFactory;
    [Inject] private UIFactory _uiFactory;

    [SerializeField] private Needle _needle;

    private readonly float _startPos = -50f;
    private readonly float _endPos = -130f;
    private float _desiredPos;
    private float _speed;
    private bool _up;
    public bool Enabled { get; private set; }

    public void Enable() => Enabled = true;

    public void Disable() => Enabled = false;

    private void Update()
    {
        if (Enabled == false)
            return;

        if (Utils.IsPointerOverUI() == false && Input.GetMouseButtonDown(0))
        {
            float launchFactor = CreateLaunchForce();
            StartCoroutine(_gameFactory.GetMainPlayer().ApplyLaunchForce(launchFactor));

            Disable();
        }
        else
        {
            if (_up)
            {
                _speed += Time.deltaTime * 150f;
                if (_speed > 179f) _up = false;
            }
            else
            {
                _speed -= Time.deltaTime * 150f;
                if (_speed < 1f) _up = true;
            }

            _desiredPos = _startPos - _endPos;
            float temp = _speed / 180;
            _needle.transform.localEulerAngles = new Vector3(_startPos - temp * _desiredPos, 0, 0);
        }
    }

    private float CreateLaunchForce()
    {
        _speed = Mathf.Abs(90f - _speed);

        return _speed switch
        {
            > 70 => 0.1f,
            > 50 => 0.65f,
            > 30 => 0.75f,
            > 10 => 0.85f,
            _ => 1.0f
        };
    }
}
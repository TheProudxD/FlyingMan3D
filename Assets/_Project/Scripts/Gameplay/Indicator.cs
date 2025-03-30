using System.Collections;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Tools;
using Reflex.Attributes;
using UnityEngine;

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
    private bool _enabled;

    public void Initialize()
    {
        _enabled = true;
    }

    private void Update()
    {
        if (_enabled == false)
            return;
        
        if (Utils.IsPointerOverUI() == false && Input.GetMouseButtonDown(0))
        {
            float launchFactor = CreateLaunchForce();
            // if (launchFactor)
            //     StartCoroutine(GameOverCo());

            StartCoroutine(_gameFactory.GetPlayer().ApplyLaunchForce(launchFactor));

            enabled = false;
            _uiFactory.GetHUD().DeactivateStartText();
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

    private IEnumerator GameOverCo()
    {
        yield return new WaitForSeconds(3);

        //UIManager.Instance.Invoke(nameof(UIManager.BadShot), 0.5f);
        _stateMachine.Enter<LoseLevelState>();
    }
}
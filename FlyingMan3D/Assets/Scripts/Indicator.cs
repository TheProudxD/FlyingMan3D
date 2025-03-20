using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] private Needle _needle;
    
    private PlayerController _player;

    private readonly float _startPos = -50f;
    private readonly float _endPos = -130f;
    private float _desiredPos;
    private float _speed;
    private bool _up;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float launchFactor = CreateLaunchForce();

            StartCoroutine(_player.ApplyLaunchForce(launchFactor));

            enabled = false;
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

        switch (_speed)
        {
            case > 70:
                UIManager.Instance.Invoke(nameof(UIManager.BadShot), 0.5f);
                GameManager.Instance.Invoke(nameof(GameManager.GameOver), 2.5f);
                return 0.1f;
            case > 50:
                return 0.65f;
            case > 30:
                return 0.75f;
            case > 10:
                return 0.85f;
            default:
                return 1.0f;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public List<PlayerController> players;

    [SerializeField] private float maxLaunchSpeed = 60f;
    [SerializeField] private float movementSpeed = 100f;
    [SerializeField] private float mobileSpeed = 10f;
    [SerializeField] private Transform capsule;
    [SerializeField] private FixedJoint joint;
    [SerializeField] public GameObject SelfHips;
    
    [HideInInspector] public bool isPassed;
    [HideInInspector] public Rigidbody[] bodies;

    private float _xValue;
    private Vector3 _initialPos;
    private float _time;
    public Animator Animator { get; private set; }

    private void Awake()
    {
        if (players == null || players.Count == 0)
        {
            players = new List<PlayerController>();
        }

        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        bodies = GetComponentsInChildren<Rigidbody>();
        _initialPos = capsule.position;
        
        players.Add(this);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (GameManager.Instance.isGameStarted)
            {
                _xValue = Input.GetAxis("Mouse X");

                foreach (Rigidbody rb in bodies)
                {
                    rb.velocity += new Vector3(_xValue, 0, 0) * Time.deltaTime * movementSpeed;
                }
            }
            else
            {
                GameManager.Instance.CloseTapText();
                GameManager.Instance.isGameStarted = true;
            }
        }

#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (GameManager.Instance.isGameStarted)
            {
                Touch touch = Input.GetTouch(0);
                TouchPhase phase = touch.phase;

                if (phase == TouchPhase.Moved)
                {
                    xValue = touch.deltaPosition.x;

                    foreach (Rigidbody rb in bodies)
                    {
                        rb.velocity += new Vector3(xValue, 0, 0) * Time.deltaTime * mobileSpeed;
                    }
                }
            }
            else
            {
                GameManager.Instance.CloseTapText();
                GameManager.Instance.isGameStarted = true;
            }
        }
#endif

        CheckForBoundaries();
    }

    private void CheckForBoundaries()
    {
        float xPos = SelfHips.transform.position.x;

        if (xPos >= 20f || xPos <= -20f)
        {
            float newX = Mathf.Sign(xPos) * -3f;
            SelfHips.GetComponent<Rigidbody>().velocity = new Vector3(newX, SelfHips.GetComponent<Rigidbody>().velocity.y, SelfHips.GetComponent<Rigidbody>().velocity.z);
        }
    }

    public IEnumerator ApplyLaunchForce(float factor)
    {
        Vector3 targetPos = _initialPos + new Vector3(0f, -1f, -4f) * factor;

        while (_time <= 0.8f)
        {
            capsule.position = Vector3.Lerp(_initialPos, targetPos, _time / 0.8f);
            _time += Time.deltaTime;
            yield return null;
        }

        _time = 0f;

        while (_time <= 0.1f)
        {
            capsule.position = Vector3.Lerp(targetPos, _initialPos, _time / 0.2f);
            _time += Time.deltaTime;
            yield return null;
        }

        Destroy(joint);

        Vector3 forceVector = new Vector3(0, factor, factor * 2f) * maxLaunchSpeed;

        foreach (Rigidbody rb in bodies)
        {
            rb.velocity = forceVector;
        }

        if (factor > 0.1f)
        {
            Spawner.Instance.SpawnObjects(bodies[0].velocity);
        }
    }
    
}

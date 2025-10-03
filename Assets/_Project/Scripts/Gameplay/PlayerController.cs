using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using Reflex.Attributes;

public class PlayerController : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    [Inject] private UIFactory _uiFactory;
    [Inject] private AudioService _audioService;
    [Inject] private WindowService _windowService;
    [Inject] private LevelResourceService _levelResourceService;
    [Inject] private IPersistentProgressService _persistentProgressService;

    private const float DIE_HEIGHT = -5;

    [field: SerializeField] public Rigidbody SelfHips { get; private set; }
    [field: SerializeField] public TrailRenderer TrailRenderer { get; private set; }

    public Rigidbody[] Bodies { get; private set; }
    private Vector3 _initialPos;
    private float _xValue;
    private float _time;
    private bool _enabled;
    private float _maxLaunchSpeed;
    private float _movementSpeed;
    private FixedJoint _joint;
    private Transform _capsule;

    public Animator Animator { get; private set; }
    public bool IsPassed { get; set; }
    public bool IsTarget { get; set; }
    public bool IsDie { get; private set; }

    private void Start()
    {
        // Input.simulateMouseWithTouches = true;
        Animator = GetComponent<Animator>();
        Bodies = GetComponentsInChildren<Rigidbody>();
    }

    public void Initialize()
    {
        _enabled = true;
        _maxLaunchSpeed = _gameFactory.GetCurrentLevel().MaxLaunchSpeed;
        _movementSpeed = _persistentProgressService.PowerupProgress.flyingControl;
    }

    public void Disable() => _enabled = false;

    public void SetInitial(FixedJoint joint, Transform capsule)
    {
        _capsule = capsule;
        _joint = joint;
        _initialPos = capsule.position;
    }

    private void Update()
    {
        if (_enabled == false)
            return;

        CheckForBoundaries();
        CheckForHeight();

        if (Utils.IsPointerOverUI())
            return;

        if (!Input.GetMouseButton(0))
            return;

        _xValue = Input.GetAxis("Mouse X");

        foreach (Rigidbody rb in Bodies)
        {
            rb.velocity += new Vector3(_xValue, 0, 0) * (Time.deltaTime * _movementSpeed);
        }
    }

    private void CheckForBoundaries()
    {
        float xPos = SelfHips.transform.position.x;
        const float DELTA = 40f;

        if (xPos is < DELTA and > -DELTA)
            return;

        float newX = Mathf.Sign(xPos) * -3f;

        SelfHips.velocity = new Vector3(newX, SelfHips.velocity.y, SelfHips.velocity.z);
    }

    public void CheckForHeight()
    {
        float yPos = SelfHips.transform.position.y;

        if (yPos < DIE_HEIGHT)
            Die();
    }

    public IEnumerator ApplyLaunchForce(float factor)
    {
        Hud hud = _uiFactory.GetHUD();
        hud.Show();
        hud.DeactivateStartText();
        _maxLaunchSpeed = _gameFactory.GetCurrentLevel().MaxLaunchSpeed;
        _movementSpeed = _persistentProgressService.PowerupProgress.flyingControl;

        _audioService.PlayLaunchSound();
        Vector3 targetPos = _initialPos + new Vector3(0f, -1f, -4f) * factor;

        while (_time <= 0.8f)
        {
            if (_capsule == null)
                yield break;

            _capsule.position = Vector3.Lerp(_initialPos, targetPos, _time / 0.8f);
            _time += Time.deltaTime;
            yield return null;
        }

        _time = 0f;

        while (_time <= 0.1f)
        {
            if (_capsule == null)
                yield break;

            _capsule.position = Vector3.Lerp(targetPos, _initialPos, _time / 0.2f);
            _time += Time.deltaTime;
            yield return null;
        }

        // if (_levelResourceService.ObservableValue.Value == 1)
        // {
        //     Task<UIContainer> tutorialTask = _windowService.Show(WindowId.Tutorial);
        //     yield return tutorialTask;
        //     (tutorialTask.Result as TutorialWindow)?.AnimateHandMovementCursor();
        // }

        Destroy(_joint);

        Vector3 forceVector = new Vector3(0, factor, factor * 2f) * _maxLaunchSpeed;

        foreach (Rigidbody rb in Bodies)
        {
            if (rb == null)
                continue;

            rb.velocity = forceVector;
            rb.AddTorque(Vector3.forward);
        }

        if (factor > 0.1f)
        {
            _gameFactory.GetSpawner()?.SpawnObjects(Bodies[0].velocity);
        }
    }

    public async void Die()
    {
        IsDie = true;
        IsTarget = false;
        if (transform != null) await _gameFactory.GetPlayerRagdoll(transform.position, Quaternion.identity);
        _gameFactory.RemovePlayer(this);
    }
}
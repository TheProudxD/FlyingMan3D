using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services.Factories;
using UnityEngine;
using Cinemachine;
using Reflex.Attributes;

public class Finish : MonoBehaviour
{
    [Inject] private StateMachine _stateMachine;
    [Inject] private GameFactory _gameFactory;

    private static readonly int Win = Animator.StringToHash("Win");

    private CinemachineVirtualCamera _finishCamera;
    private bool _canCalculate;
    private bool _isPlayerEntered;

    private void Start() => _finishCamera = GameObject.Find("FinishCamera").GetComponent<CinemachineVirtualCamera>();

    private void Update()
    {
        if (!_isPlayerEntered) return;

        if (_gameFactory.Players.Count == 0 && _gameFactory.Enemies.Count >= 0)
        {
            foreach (EnemyFinish item in _gameFactory.Enemies)
            {
                item.Animator.SetBool(Win, true);
                Destroy(item.GetComponent<Rigidbody>());
            }

            _stateMachine.Enter<LoseLevelState>();
            _isPlayerEntered = false;
        }
        else if (_gameFactory.Enemies.Count == 0 && _gameFactory.Players.Count > 0)
        {
            foreach (PlayerController item in _gameFactory.Players)
            {
                item.Animator.SetBool(Win, true);
                Destroy(item.GetComponent<Rigidbody>());
            }

            _stateMachine.Enter<WinLevelState>();
            _isPlayerEntered = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.CompareTag("Player") == false)
            return;

        if (!_canCalculate)
        {
            _canCalculate = true;

            _finishCamera.Priority = 15;
            _finishCamera.transform.position = new Vector3(0, 23, transform.position.z - 30f);

            foreach (EnemyFinish e in _gameFactory.Enemies)
            {
                e.SetAttackState();
            }

            _isPlayerEntered = true;
        }

        Transform root = other.gameObject.transform.root;
        root.tag = "FreePlayer";

        root.GetComponent<PlayerMoveToFinish>().enabled = true;
        root.GetComponent<CapsuleCollider>().enabled = true;

        var hips = root.GetComponent<PlayerController>().SelfHips;
        var rg = root.gameObject.AddComponent<Rigidbody>();
        rg.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        root.GetComponent<Animator>().enabled = true;
        Collider[] colliders = root.GetComponentsInChildren<Collider>();

        for (int i = 1; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        Destroy(hips.GetComponent<TrailRenderer>());

        root.position = new Vector3(Random.Range(-8, 8), transform.position.y + root.transform.position.y,
            transform.position.z + Random.Range(-8f, 8f));

        hips.transform.localPosition = Vector3.zero;
    }
}
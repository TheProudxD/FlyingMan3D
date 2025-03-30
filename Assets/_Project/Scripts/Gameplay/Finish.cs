using System.Collections;
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
    private bool _attack;
    private bool _isGameOver;
    private WaitForSeconds _waiter;

    private void Start()
    {
        _finishCamera = GameObject.Find("FinishCamera").GetComponent<CinemachineVirtualCamera>();
        _waiter = new WaitForSeconds(2.5f);
    }

    private void Update()
    {
        if (_isGameOver)
            return;

        if (_gameFactory.Players.Count == 0 && _gameFactory.Enemies.Count >= 0)
        {
            foreach (Enemy item in _gameFactory.Enemies)
            {
                item.Animator.SetBool(Win, true);
                Destroy(item.GetComponent<Rigidbody>());
            }

            _stateMachine.Enter<LoseLevelState>();
        }
        else if (_gameFactory.Enemies.Count == 0 && _gameFactory.Players.Count > 0)
        {
            foreach (PlayerController item in _gameFactory.Players)
            {
                item.Animator.SetBool(Win, true);
                Destroy(item.GetComponent<Rigidbody>());
            }

            StartCoroutine(WinCoroutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.CompareTag("Player") == false)
            return;

        StartCoroutine(SetAttackState(other));

        if (_attack)
            return;

        _attack = true;
        _finishCamera.Priority = 15;
        _finishCamera.transform.position = new Vector3(0, 25, transform.position.z - 30f);

        foreach (Enemy e in _gameFactory.Enemies)
        {
            e.SetAttackState();
        }
    }

    private IEnumerator WinCoroutine()
    {
        _isGameOver = true;
        ParticleSystem winParticle = _gameFactory.GetSpawner().WinParticle;
        winParticle.transform.position = transform.position;
        winParticle.Play();
        yield return new WaitForSeconds(2);

        _stateMachine.Enter<WinLevelState>();
    }

    private IEnumerator SetAttackState(Collider other)
    {
        Transform root = other.gameObject.transform.root;
        root.tag = "FreePlayer";

        var playerController = root.GetComponent<PlayerController>();

        float maxVelocity = 4;

        foreach (Rigidbody r in playerController.Bodies)
        {
            r.velocity /= maxVelocity;

            // Utils.LerpFunction(1, x =>
            // {
            //     if (r.velocity.x > maxVelocity)
            //     {
            //         r.velocity = maxVelocity * new Vector3(Math.Abs(x), Math.Abs(x), Math.Abs(x));
            //     }
            // });
        }

        Rigidbody hips = playerController.SelfHips;
        Destroy(hips.GetComponent<TrailRenderer>());
        yield return _waiter;

        var rg = root.gameObject.AddComponent<Rigidbody>();
        rg.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        root.GetComponent<PlayerFinishMover>().enabled = true;

        root.GetComponent<CapsuleCollider>().enabled = true;

        root.GetComponent<Animator>().enabled = true;
        Collider[] colliders = root.GetComponentsInChildren<Collider>();

        for (int i = 1; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        //root.position = new Vector3(Random.Range(-8, 8), transform.position.y + root.transform.position.y, transform.position.z + Random.Range(-8f, 8f));
        root.position = new Vector3(hips.transform.position.x, hips.transform.position.y, hips.transform.position.z);
        hips.transform.localPosition = Vector3.zero;
    }
}
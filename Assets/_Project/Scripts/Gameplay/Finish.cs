using System.Collections;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using UnityEngine;
using Reflex.Attributes;

public class Finish : MonoBehaviour
{
    [Inject] private StateMachine _stateMachine;
    [Inject] private GameFactory _gameFactory;
    [Inject] private AudioService _audioService;

    private bool _attack;
    private bool _isGameOver;
    private WaitForSeconds _waiter;

    private void Start()
    {
        _waiter = new WaitForSeconds(1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.CompareTag("Player") == false)
            return;

        StartCoroutine(SetAttackState(other));

        if (_attack)
            return;
        
        _audioService.PlayHitSound();

        _attack = true;
        _gameFactory.SetFinishCamera(transform.position.z);

        foreach (Enemy e in _gameFactory.Enemies)
        {
            if (e.gameObject.activeInHierarchy)
                e.SetAttackState();
        }
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
        playerController.Disable();
        Destroy(playerController.TrailRenderer);
        Time.timeScale = 0.5f;
        yield return _waiter;

        Time.timeScale = 1;

        if (root == null || root.gameObject == null)
            yield break;

        var rg = root.gameObject.AddComponent<Rigidbody>();
        rg.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        PlayerFinishMover playerFinishMover = root.GetComponent<PlayerFinishMover>();
        playerFinishMover.Initialize();

        root.GetComponent<CapsuleCollider>().enabled = true;

        playerController.Animator.enabled = true;

        Rigidbody[] rgs = playerController.Bodies;

        for (int i = 0; i < rgs.Length; i++)
        {
            rgs[i].isKinematic = true;
            rgs[i].useGravity = false;
        }
        
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
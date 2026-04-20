using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using _Project.Scripts.Gameplay;

public class MultiplierRing : RingBase
{
    [Inject] private PlayerFactory _playerFactory;
    private bool _firstPlayer;
    private int _playerCount;

    protected override string Key => "x";

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player"))
            return;

        if (!_firstPlayer)
        {
            _playerCount = FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
            _firstPlayer = true;
        }

        var playerController = root.GetComponent<PlayerController>();

        if (playerController.IsPassed || _playerCount <= 0)
            return;

        playerController.IsPassed = true;

        for (int i = 0; i < Effect - 1; i++)
        {
            _playerFactory.GetNewPlayer();
        }

        _playerCount--;

        AudioService.PlayRingCollideSound();
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Player"))
        {
            if (root.GetComponent<PlayerController>().IsPassed)
            {
                root.GetComponent<PlayerController>().IsPassed = false;
            }
        }

        // if (!other.transform.root.TryGetComponent(out PlayerController player))
        //     return;
        //
        // if (player.IsPassed)
        //     player.IsPassed = false;
    }
}

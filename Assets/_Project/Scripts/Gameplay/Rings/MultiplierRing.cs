using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class MultiplierRing : RingBase
{
    private bool _firstPlayer;
    private int _playerCount;
    
    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.TryGetComponent(out PlayerController player))
            return;

        if (!_firstPlayer)
        {
            _playerCount = GameFactory.Players.Count;
            _firstPlayer = true;
        }

        if (player.IsPassed || _playerCount <= 0)
            return;

        player.IsPassed = true;

        for (int i = 0; i < Effect - 1; i++)
        {
            GameFactory.GetNewPlayer(root);
        }

        _playerCount--;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController player))
            return;

        if (player.IsPassed)
            player.IsPassed = false;
    }
}
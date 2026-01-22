using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

public class ReducerRing : RingBase
{
    [Inject] private PlayerFactory _playerFactory;
    
    private bool _reductionHappened;

    protected override string Key => "-";

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player")) return;

        if (_reductionHappened) return;

        var players = _playerFactory.GetAllPlayers();

        for (int i = 0; i < Effect && players.Count > 1; i++)
        {
            _playerFactory.DestroyLastPlayer();
        }

        _reductionHappened = true;

        AudioService.PlayRingCollideSound();
    }
}
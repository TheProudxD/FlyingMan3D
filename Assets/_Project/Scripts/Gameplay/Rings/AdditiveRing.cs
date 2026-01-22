using System;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

public class AdditiveRing : RingBase
{
    [Inject] private PlayerFactory _playerFactory;
    
    private bool _additionHappened;

    protected override string Key => "+";

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player"))
            return;

        if (_additionHappened)
            return;

        for (int i = 0; i < Effect; i++)
            _playerFactory.GetNewPlayer();

        _additionHappened = true;
        
        AudioService.PlayRingCollideSound();
    }
}
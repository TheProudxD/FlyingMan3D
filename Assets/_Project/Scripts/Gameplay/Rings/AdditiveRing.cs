using System;
using UnityEngine;

public class AdditiveRing : RingBase
{
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
            GameFactory.GetNewPlayer();

        _additionHappened = true;
        
        AudioService.PlayRingCollideSound();
    }
}
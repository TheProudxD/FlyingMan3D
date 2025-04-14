using UnityEngine;

public class ReducerRing : RingBase
{
    private bool _reductionHappened;

    protected override string Key => "-";

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player")) return;

        if (_reductionHappened) return;

        for (int i = 0; i < Effect && GameFactory.Players.Count > 1; i++)
        {
            GameFactory.DestroyLastPlayer();
        }

        _reductionHappened = true;

        AudioService.PlayRingCollideSound();
    }
}
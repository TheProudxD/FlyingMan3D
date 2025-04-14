using UnityEngine;

public class DividerRing : RingBase
{
    private bool _reductionHappened;

    protected override string Key => "/";

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player")) return;

        if (_reductionHappened) return;

        int players = GameFactory.Players.Count / Effect;

        for (int i = 0; i < players && GameFactory.Players.Count > 1; i++)
        {
            GameFactory.DestroyLastPlayer();
        }

        _reductionHappened = true;
        AudioService.PlayRingCollideSound();
    }
}
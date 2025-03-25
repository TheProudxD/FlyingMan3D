using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReducerRing : MonoBehaviour
{
    [FormerlySerializedAs("reductionFactor")] [SerializeField] public int ReductionFactor;
    private bool _reductionHappened;

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player")) return;

        if (_reductionHappened) return;

        for (int i = 0; i < ReductionFactor && PlayerController.players.Count > 1; i++)
        {
            Destroy(PlayerController.players[^1].gameObject);
            PlayerController.players.RemoveAt(PlayerController.players.Count - 1);
        }

        _reductionHappened = true;
    }
}

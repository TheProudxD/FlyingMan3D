using System;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ReducerRing : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    
    [field: SerializeField] public TMP_Text Text { get; private set; }
    
    [FormerlySerializedAs("reductionFactor")] [SerializeField] public int ReductionFactor;
    
    private bool _reductionHappened;

    private void Awake() => Text = GetComponentInChildren<TMP_Text>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player")) return;

        if (_reductionHappened) return;

        for (int i = 0; i < ReductionFactor && _gameFactory.Players.Count > 1; i++)
        {
            _gameFactory.DestroyLastPlayer();
        }

        _reductionHappened = true;
    }
}

using System;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ReducerRing : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    
    [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
    
    [FormerlySerializedAs("reductionFactor")] [SerializeField] public int ReductionFactor;
    
    private bool _reductionHappened;

    private void Awake() => Text = GetComponentInChildren<TextMeshProUGUI>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player")) return;

        if (_reductionHappened) return;

        for (int i = 0; i < ReductionFactor && _gameFactory.players.Count > 1; i++)
        {
            Destroy(_gameFactory.players[^1].gameObject);
            _gameFactory.players.RemoveAt(_gameFactory.players.Count - 1);
        }

        _reductionHappened = true;
    }
}

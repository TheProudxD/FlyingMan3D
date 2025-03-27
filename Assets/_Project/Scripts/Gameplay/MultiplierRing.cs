using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class MultiplierRing : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;

    [field: SerializeField] public TMP_Text Text { get; private set; }
    public int Multiplier { get; set; }

    private bool _firstPlayer;
    private int _playerCount;

    private void Awake() => Text = GetComponentInChildren<TMP_Text>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.TryGetComponent(out PlayerController player))
            return;

        if (!_firstPlayer)
        {
            _playerCount = _gameFactory.Players.Count;
            _firstPlayer = true;
        }

        if (player.IsPassed || _playerCount <= 0)
            return;

        player.IsPassed = true;

        for (int i = 0; i < Multiplier - 1; i++)
        {
            _gameFactory.GetNewPlayer(root);
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
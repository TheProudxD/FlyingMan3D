using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AdditiveRing : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    
    [field: SerializeField] public TextMeshProUGUI Text { get; private set; }

    public int Addition { get; set; }

    private bool _additionHappened;
    
    private void Awake() => Text = GetComponentInChildren<TextMeshProUGUI>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player"))
            return;

        if (_additionHappened)
            return;

        for (int i = 0; i < Addition; i++)
            _gameFactory.GetNewPlayer(root);

        _additionHappened = true;
    }
}
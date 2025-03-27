using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class AdditiveRing : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    
    [field: SerializeField] public TMP_Text Text { get; private set; }

    public int Addition { get; set; }

    private bool _additionHappened;
    
    private void Awake() => Text = GetComponentInChildren<TMP_Text>();

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
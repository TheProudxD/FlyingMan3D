using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class RingBase : MonoBehaviour
{
    [Inject] protected GameFactory GameFactory;
    [field: SerializeField] public TMP_Text Text { get; private set; }
    public int Effect { get; set; }

    private void Awake() => Text = GetComponentInChildren<TMP_Text>();
}

public class AdditiveRing : RingBase
{
    private bool _additionHappened;

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
    }
}
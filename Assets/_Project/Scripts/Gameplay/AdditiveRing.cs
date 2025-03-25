using UnityEngine;

public class AdditiveRing : MonoBehaviour
{
    [SerializeField] public int addition;

    private bool _additionHappened;

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player"))
            return;

        if (_additionHappened)
            return;

        for (int i = 0; i < addition; i++)
            Ring.DuplicatePlayer(root);

        _additionHappened = true;
    }
}
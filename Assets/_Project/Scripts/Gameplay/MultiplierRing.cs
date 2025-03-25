using UnityEngine;

public class MultiplierRing : MonoBehaviour
{
    public int multiplier;

    private bool _firstPlayer;
    private int _playerCount;

    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player"))
            return;

        if (!_firstPlayer)
        {
            _playerCount = FindObjectsOfType<PlayerController>().Length;
            _firstPlayer = true;
        }

        if (root.GetComponent<PlayerController>().isPassed || _playerCount <= 0)
            return;

        root.GetComponent<PlayerController>().isPassed = true;

        for (int i = 0; i < multiplier - 1; i++)
        {
            Ring.DuplicatePlayer(root);
        }

        _playerCount--;
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (!root.CompareTag("Player"))
            return;

        if (root.GetComponent<PlayerController>().isPassed)
            root.GetComponent<PlayerController>().isPassed = false;
    }
}
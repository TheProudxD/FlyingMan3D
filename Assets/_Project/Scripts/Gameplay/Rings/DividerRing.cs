using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class DividerRing : RingBase
    {
        private bool _reductionHappened;

        protected override string Key => "/";

        private void OnTriggerEnter(Collider other)
        {
            GameObject root = other.transform.root.gameObject;

            if (!root.CompareTag("Player"))
                return;

            if (_reductionHappened)
                return;

            var allPlayers = PlayerFactory.GetAllPlayers();
            int players = allPlayers.Count / Effect;

            for (int i = 0; i < players && allPlayers.Count > 1; i++)
            {
                PlayerFactory.DestroyLastPlayer();
            }

            _reductionHappened = true;
            AudioService.PlayRingCollideSound();
        }
    }
}

using Reflex.Attributes;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class Finish : MonoBehaviour
    {
        [Inject] private PlayerFinishCombatService _playerFinishCombatService;
        [Inject] private PlayerFinishTransitionService _playerFinishTransitionService;

        private bool _combatStarted;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.CompareTag("Player") == false)
                return;

            if (!other.gameObject.transform.root.TryGetComponent(out PlayerController playerController))
                return;

            bool shouldPlayTransitionSlowMotion = !_combatStarted;
            if (shouldPlayTransitionSlowMotion)
            {
                _playerFinishCombatService.StartCombat(transform.position.z);
                _combatStarted = true;
            }

            _playerFinishTransitionService.TransitionPlayerAsync(playerController, shouldPlayTransitionSlowMotion)
                .Forget();
        }
    }
}

using UnityEngine;
using _Project.Scripts.Infrastructure.Services;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay
{
    public class PlayerFinishTransitionService : IService
    {
        private const float MaxVelocityDivider = 4f;

        public void BeginTransition(PlayerController playerController)
        {
            if (playerController == null)
                return;

            Transform root = playerController.transform;
            root.tag = "FreePlayer";

            foreach (Rigidbody body in playerController.Bodies)
            {
                if (body != null)
                    body.linearVelocity /= MaxVelocityDivider;
            }

            playerController.Disable();

            if (playerController.TrailRenderer != null)
                Object.Destroy(playerController.TrailRenderer);
        }

        public PlayerFinishMover CompleteTransition(PlayerController playerController)
        {
            if (playerController == null)
                return null;

            Transform root = playerController.transform;
            Rigidbody hips = playerController.SelfHips;

            if (root == null || hips == null)
                return null;

            Rigidbody rootRigidbody = root.GetComponent<Rigidbody>();
            if (rootRigidbody == null)
                rootRigidbody = root.gameObject.AddComponent<Rigidbody>();

            rootRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (!root.TryGetComponent(out PlayerFinishMover playerFinishMover))
                return null;

            if (root.TryGetComponent(out CapsuleCollider capsuleCollider))
                capsuleCollider.enabled = true;

            if (playerController.Animator != null)
                playerController.Animator.enabled = true;

            Rigidbody[] rigidbodies = playerController.Bodies;
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                if (rigidbodies[i] == null)
                    continue;

                rigidbodies[i].isKinematic = true;
                rigidbodies[i].useGravity = false;
            }

            Collider[] colliders = root.GetComponentsInChildren<Collider>();
            for (int i = 1; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            root.position = hips.transform.position;
            hips.transform.localPosition = Vector3.zero;
            return playerFinishMover;
        }
    }
}

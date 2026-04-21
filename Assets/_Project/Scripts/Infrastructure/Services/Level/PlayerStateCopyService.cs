using _Project.Scripts.Gameplay;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Level
{
    public class PlayerStateCopyService : IService
    {
        public void CopyPlayerState(PlayerController sourcePlayer, PlayerController targetPlayer)
        {
            if (sourcePlayer == null || targetPlayer == null)
                return;

            targetPlayer.Initialize();
            CopyTransformData(sourcePlayer.transform, targetPlayer.transform);
        }

        private void CopyTransformData(Transform sourceTransform, Transform targetTransform)
        {
            if (sourceTransform.childCount != targetTransform.childCount)
            {
                UnityEngine.Debug.LogError("Players have different hierarchies!");
            }

            for (int i = 0; i < sourceTransform.childCount; i++)
            {
                Transform source = sourceTransform.GetChild(i);
                Transform target = targetTransform.GetChild(i);

                Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();

                if (targetRigidbody != null)
                {
                    Rigidbody sourceRigidbody = source.GetComponent<Rigidbody>();

                    if (sourceRigidbody != null)
                        targetRigidbody.linearVelocity = sourceRigidbody.linearVelocity;
                }

                CopyTransformData(source, target);
            }
        }
    }
}

using _Project.Scripts.Infrastructure.Services.Config;
using UnityEngine;

namespace _Project.Scripts.UI
{
    [CreateAssetMenu(menuName = "Configs/Coin Reward Animation Config", fileName = "CoinRewardAnimationConfig",
        order = 0)]
    public class CoinRewardAnimationConfig : Config
    {
        [field: SerializeField] public float ScaleUpDuration { get; private set; } = 0.3f;
        [field: SerializeField] public float MoveDuration { get; private set; } = 0.8f;
        [field: SerializeField] public float RotationDuration { get; private set; } = 0.5f;
        [field: SerializeField] public float ScaleDownDuration { get; private set; } = 0.3f;
        [field: SerializeField] public float DelayStep { get; private set; } = 0.1f;
        [field: SerializeField] public float StartDelay { get; private set; } = 0f;
    }
}
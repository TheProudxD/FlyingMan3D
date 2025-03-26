using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Review
{
    [CreateAssetMenu(menuName = "Configs/ReviewData", fileName = "ReviewData", order = 0)]
    public class ReviewData : Config.Config
    {
        [field: SerializeField] public int Reward { get; private set; }
        [field: SerializeField] public int RequiredLevel { get; private set; }
    }
}
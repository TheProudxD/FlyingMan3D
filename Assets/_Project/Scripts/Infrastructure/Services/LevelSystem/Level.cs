using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.LevelSystem
{
    [CreateAssetMenu(menuName = "Create Level", fileName = "Level", order = 0)]
    public class Level : ScriptableObject
    {
        [field: SerializeField] public UpperRing[] Rings { get; private set; }
        [field: SerializeField] public float TimeDif { get; private set; }
        [field: SerializeField] public float MaxLaunchSpeed { get; private set; }
        [field: SerializeField] public float StopDistance { get; private set; }
        [field: SerializeField] public int MoneyReward { get; private set; }
        [field: SerializeField] public int BarrelAmount { get; private set; }
        [field: SerializeField] public List<EnemyData> Enemies { get; private set; }
    }
}
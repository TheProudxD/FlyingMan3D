using _Project.Scripts.Infrastructure.Services.Config;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    [CreateAssetMenu(menuName = "Configs/GameData Config", fileName = "GameData", order = 0)]
    public class GameData : Config
    {
        [field: SerializeField] public string Version { get; private set; }
    }
}
using _Project.Scripts.Infrastructure.Services.Config;
using UnityEngine;

namespace _Project.Scripts.UI
{
    [CreateAssetMenu(menuName = "Configs/Win Images Config", fileName = "WinImagesConfig",
        order = 0)]
    public class WinImagesConfig : Config
    {
        [field: SerializeField] public Sprite[] Sprites { get; private set; }
    }
}
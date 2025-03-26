using _Project.Scripts.Infrastructure.Services.Config;
using UnityEngine;

namespace _Project.Scripts.UI
{
    [CreateAssetMenu(menuName = "Configs/Win Window Animations Config", fileName = "WinWindowAnimationsConfig",
        order = 0)]
    public class WinWindowAnimationsConfig : Config
    {
        [field: SerializeField] public float TimeBeforeShowRestartButton { get; private set; } = 1.75f;
        [field: SerializeField] public float ShowDuration { get; private set; } = 0.75f;
        [field: SerializeField] public float HideDuration { get; private set; } = 0.2f;
        [field: SerializeField] public float FromScale { get; private set; } = 1f;
        [field: SerializeField] public float ToScale { get; private set; } = 1.075f;
        [field: SerializeField] public float Duration { get; private set; } = 0.65f;
    }
}
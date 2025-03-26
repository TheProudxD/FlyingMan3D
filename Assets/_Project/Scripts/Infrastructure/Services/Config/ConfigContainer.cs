using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Config
{
    [CreateAssetMenu(menuName = "Config Container", fileName = "ConfigContainer", order = 0)]
    public class ConfigContainer : ScriptableObject
    {
        [field: SerializeField] public Config[] Configs { get; private set; }
    }
}
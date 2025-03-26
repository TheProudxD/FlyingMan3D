using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Services.Config;
using UnityEngine;

namespace _Project.Scripts.UI
{
    [CreateAssetMenu(fileName = "WindowStaticData Config", menuName = "Configs/Window static data", order = 0)]
    public class WindowStaticData : ScriptableObject
    {
        public List<WindowConfig> Configs;
    }
}
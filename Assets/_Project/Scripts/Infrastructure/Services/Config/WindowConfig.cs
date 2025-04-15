using System;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Infrastructure.Services.Config
{
    [Serializable]
    public class WindowConfig
    {
        public WindowId Id;
        public UIContainer Prefab;
    }
}
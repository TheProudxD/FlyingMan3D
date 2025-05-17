using System;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Infrastructure.Services.Config
{
    [Serializable]
    public class WindowConfig
    {
        public WindowId Id;
        public AssetReferenceGameObject Prefab;
    }
}
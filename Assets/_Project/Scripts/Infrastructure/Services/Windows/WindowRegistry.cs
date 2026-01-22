using System;
using System.Collections.Generic;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.Services.Windows
{
    public class WindowRegistry : IService
    {
        private readonly Dictionary<WindowId, Func<UniTask<UIContainer>>> _creators = new();
        
        public void RegisterFactory(WindowId windowId, Func<UniTask<UIContainer>> factory)
        {
            if (_creators.ContainsKey(windowId))
            {
                UnityEngine.Debug.LogWarning($"Factory for window {windowId} is already registered. Overwriting.");
            }

            _creators[windowId] = factory;
        }
        
        public Func<UniTask<UIContainer>> GetFactory(WindowId windowId)
        {
            if (_creators.TryGetValue(windowId, out var factory)) 
                return factory;
            
            throw new ArgumentOutOfRangeException(nameof(windowId), windowId, $"No factory registered for window {windowId}");
        }
        
        public bool IsRegistered(WindowId windowId) => _creators.ContainsKey(windowId);
    }
}
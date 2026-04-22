using System.Collections.Generic;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services.Windows
{
    public class WindowService : IService
    {
        private readonly WindowRegistry _windowRegistry;
        private readonly WindowResourceManager _resourceManager;
        
        private readonly Dictionary<WindowId, UIContainer> _openedWindows = new();
        private readonly Dictionary<UIContainer, WindowId> _openedWindowIds = new();

        public WindowService(WindowRegistry windowRegistry, WindowResourceManager resourceManager)
        {
            _windowRegistry = windowRegistry;
            _resourceManager = resourceManager;
        }

        public async UniTask<UIContainer> Show(WindowId windowId)
        {
            if (_openedWindows.TryGetValue(windowId, out var container))
            {
                if (container != null)
                    return container;

                _openedWindows.Remove(windowId);
            }

            var creator = _windowRegistry.GetFactory(windowId);
            if (creator == null)
            {
                UnityEngine.Debug.LogError($"No creator registered for window: {windowId}");
                return null;
            }

            UIContainer window = await creator();

            if (window == null)
            {
                UnityEngine.Debug.LogError($"Failed to create window component for {windowId}");
                return null;
            }

            _openedWindows[windowId] = window;
            _openedWindowIds[window] = windowId;

            window.Show();
            return window;
        }

        public void Hide(WindowId windowId)
        {
            if (_openedWindows.Remove(windowId, out UIContainer window))
            {
                if (window != null)
                {
                    _openedWindowIds.Remove(window);
                    Object.Destroy(window.gameObject);
                }

                _resourceManager.ReleaseAsset(windowId);
            }
        }

        public void Hide(UIContainer windowBase)
        {
            if (windowBase == null)
                return;

            if (_openedWindowIds.TryGetValue(windowBase, out WindowId windowId))
            {
                Hide(windowId);
            }
        }
    }
}

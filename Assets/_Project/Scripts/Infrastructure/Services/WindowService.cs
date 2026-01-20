using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services
{
    public class WindowService : IService
    {
        private readonly ConfigService _configService;
        private readonly Dictionary<WindowId, UIContainer> _openedWindows = new();
        private readonly Dictionary<UIContainer, WindowId> _openedWindowIds = new();
        private readonly Dictionary<WindowId, Func<UniTask<UIContainer>>> _creators;

        public WindowService(UIFactory uiFactory, ConfigService configService)
        {
            _configService = configService;
            _creators = new Dictionary<WindowId, Func<UniTask<UIContainer>>>
            {
                [WindowId.Pause] = uiFactory.CreatePauseWindow,
                [WindowId.Lose] = uiFactory.CreateLoseWindow,
                [WindowId.Tutorial] = uiFactory.CreateTutorialWindow,
                [WindowId.Leaderboard] = uiFactory.CreateLeaderboardWindow,
                [WindowId.Win] = uiFactory.CreateWinWindow,
                [WindowId.HUD] = uiFactory.CreateHUD,
            };
        }

        public async UniTask<UIContainer> Show(WindowId windowId)
        {
            if (_openedWindows.ContainsKey(windowId))
            {
                UnityEngine.Debug.LogError("Double opening window");
            }
            else
            {
                if (!_creators.TryGetValue(windowId, out var creator))
                    throw new ArgumentOutOfRangeException(nameof(windowId), windowId, null);

                UIContainer window = await creator.Invoke();

                if (window == null)
                {
                    UnityEngine.Debug.LogError($"There is no window component on {windowId} window");
                }

                _openedWindows[windowId] = window;
                _openedWindowIds[window] = windowId;
            }

            UIContainer windowBase = _openedWindows[windowId];
            windowBase.Show();
            return windowBase;
        }

        public void Hide(WindowId windowId)
        {
            if (_openedWindows.Remove(windowId, out UIContainer window))
            {
                _openedWindowIds.Remove(window);
                Object.Destroy(window.gameObject);
                _configService.ForWindow(windowId).Prefab.ReleaseAsset();
            }
            else
            {
                UnityEngine.Debug.LogError("Trying to hide already hidden window.");
            }
        }

        public void Hide(WindowBase windowBase)
        {
            if (_openedWindowIds.TryGetValue(windowBase, out WindowId windowId))
            {
                Hide(windowId);
            }
            else
            {
                UnityEngine.Debug.LogError("Trying to hide already hidden window.");
            }
        }
    }
}
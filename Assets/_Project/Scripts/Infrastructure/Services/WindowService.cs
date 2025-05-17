using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services
{
    public class WindowService : IService
    {
        private readonly UIFactory _uiFactory;
        private readonly ConfigService _configService;
        private readonly Dictionary<WindowId, UIContainer> _openedWindows = new();

        public WindowService(UIFactory uiFactory, ConfigService configService)
        {
            _uiFactory = uiFactory;
            _configService = configService;
        }

        public async Task<UIContainer> Show(WindowId windowId)
        {
            if (_openedWindows.ContainsKey(windowId))
            {
                UnityEngine.Debug.LogError("Double opening window");
            }
            else
            {
                UIContainer window = windowId switch
                {
                    WindowId.Unknown => throw new ArgumentOutOfRangeException(nameof(windowId), windowId, null),
                    WindowId.Pause => await _uiFactory.CreatePauseWindow(),
                    WindowId.Lose => await _uiFactory.CreateLoseWindow(),
                    WindowId.Tutorial => await _uiFactory.CreateTutorialWindow(),
                    WindowId.Leaderboard => await _uiFactory.CreateLeaderboardWindow(),
                    WindowId.Win => await _uiFactory.CreateWinWindow(),
                    WindowId.HUD => await _uiFactory.CreateHUD(),
                    _ => throw new ArgumentOutOfRangeException(nameof(windowId), windowId, null)
                };

                if (window == null)
                {
                    UnityEngine.Debug.LogError($"There is no window component on {windowId} window");
                }

                _openedWindows[windowId] = window;
            }

            UIContainer windowBase = _openedWindows[windowId];
            windowBase.Show();
            return windowBase;
        }

        public void Hide(WindowId windowId)
        {
            if (_openedWindows.Remove(windowId, out UIContainer window))
            {
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
            if (_openedWindows.ContainsValue(windowBase))
            {
                WindowId windowId = _openedWindows.First(w => w.Value == windowBase).Key;
                Hide(windowId);
            }
            else
            {
                UnityEngine.Debug.LogError("Trying to hide already hidden window.");
            }
        }
    }
}
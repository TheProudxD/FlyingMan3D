using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI.Windows;

namespace _Project.Scripts.Infrastructure.Services
{
    public class WindowService : IService
    {
        private readonly UIFactory _uiFactory;
        private readonly Dictionary<WindowId, WindowBase> _openedWindows = new();

        public WindowService(UIFactory uiFactory) => _uiFactory = uiFactory;

        public void Show(WindowId windowId)
        {
            if (_openedWindows.ContainsKey(windowId) == false)
            {
                WindowBase window = windowId switch
                {
                    WindowId.Unknown => throw new ArgumentOutOfRangeException(nameof(windowId), windowId, null),
                    WindowId.Pause => _uiFactory.CreatePauseWindow(),
                    WindowId.Lose => _uiFactory.CreateLoseWindow(),
                    //WindowId.Tutorial => _uiFactory.CreateTutorialWindow(),
                    WindowId.Leaderboard => _uiFactory.CreateLeaderboardWindow(),
                    WindowId.Win => _uiFactory.CreateWinWindow(),
                    _ => throw new ArgumentOutOfRangeException(nameof(windowId), windowId, null)
                };

                if (window == null)
                {
                    UnityEngine.Debug.LogError($"There is no window component on {windowId} window");
                }

                _openedWindows[windowId] = window;
            }

            _openedWindows[windowId].Show();
        }

        public void Hide(WindowId windowId)
        {
            if (_openedWindows.TryGetValue(windowId, out WindowBase window))
            {
                //Object.Destroy(window.gameObject);
                window.Hide();
            }
            else
            {
                UnityEngine.Debug.LogError("Закрытие не открытого окна.");
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
                UnityEngine.Debug.LogError("Закрытие не открытого окна.");
            }
        }
    }
}
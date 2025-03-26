using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI.Buttons;
using _Project.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class Hud : MonoBehaviour, IUIContainer
    {
        [SerializeField] private PauseButton _pauseButton;
        
        public void Initialize()
        {
            Hide();
        }

        public void Show()
        {
            _pauseButton.Activate();
        }

        public void Hide()
        {
            _pauseButton.Deactivate();
        }
    }
}
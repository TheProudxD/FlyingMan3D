using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.UI.Windows;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Buttons
{
    public class WindowButton : ButtonBase
    {
        [Inject] private WindowService _windowService;
        
        [SerializeField] private WindowId _windowId;

        protected override async void OnClick() => await _windowService.Show(_windowId);
    }
}
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Buttons
{
    public class WindowButton : ButtonBase
    {
        [Inject] private WindowService _windowService;
        
        [SerializeField] private WindowId _windowId;

        protected override void OnClick() => OnClickAsync().Forget();

        private async UniTask OnClickAsync() => await _windowService.Show(_windowId);
    }
}
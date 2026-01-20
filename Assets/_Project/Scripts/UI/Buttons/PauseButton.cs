using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Buttons
{
    public class PauseButton : ButtonBase
    {
        [Inject] private WindowService _windowService;
        [Inject] private AdsService _adsService;

        protected override void OnClick() => OnClickAsync().Forget();

        private async UniTask OnClickAsync()
        {
            _adsService.PlayInterstitial();
            await _windowService.Show(WindowId.Pause);
        }
    }
}
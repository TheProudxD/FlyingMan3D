using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.UI.Windows;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Buttons
{
    public class PauseButton : ButtonBase
    {
        [Inject] private WindowService _windowService;
        [Inject] private AdsService _adsService;

        protected override async void OnClick()
        {
            _adsService.PlayInterstitial();
            await _windowService.Show(WindowId.Pause);
        }
    }
}
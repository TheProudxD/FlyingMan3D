using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Windows;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Buttons
{
    public class LeaderboardButton : ButtonBase
    {
        [Inject] private WindowService _windowService;

        protected override void OnClick() => OnClickAsync().Forget();

        private async UniTask OnClickAsync() => await _windowService.Show(WindowId.Leaderboard);
    }
}
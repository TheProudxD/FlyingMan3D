using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.UI.Windows;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Buttons
{
    public class LeaderboardButton : ButtonBase
    {
        [Inject] private WindowService _windowService;

        protected override async void OnClick() => await _windowService.Show(WindowId.Leaderboard);
    }
}
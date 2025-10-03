using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;
using YG;

namespace _Project.Scripts.UI.Buttons
{
    public class MoreGamesButton : ButtonBase
    {
        [Inject] private MetricService _metricService;
        
        protected override void OnClick()
        {
            // YG2.OnDeveloperURL();
            _metricService.OpenedMoreGames();
        }
    }
}
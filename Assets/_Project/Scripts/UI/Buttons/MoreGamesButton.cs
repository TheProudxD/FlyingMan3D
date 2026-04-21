using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.UI.Buttons
{
    public class MoreGamesButton : ButtonBase
    {
        private const string StubPrefix = "[MoreGamesButton:STUB]";

        [Inject] private MetricService _metricService;
        
        protected override void OnClick()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.Log($"{StubPrefix} Developer URL is not integrated.");
#endif
            _metricService.OpenedMoreGames();
        }
    }
}

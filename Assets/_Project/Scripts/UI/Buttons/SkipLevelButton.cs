using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools.Extensions;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Buttons
{
    public class SkipLevelButton : ButtonBase
    {
        [Inject] private MetricService _metricService;
        [Inject] private StateMachine _stateMachine;
        [Inject] private AdsService _adsService;
        [Inject] private GameFactory _gameFactory;
        [Inject] private LevelResourceService _levelResourceService;

        protected override void OnClick()
        {
            _adsService.PlayRewardedVideo("skipLevelForAd", () =>
            {
                gameObject.Deactivate();
                _levelResourceService.Increase(this);
                _metricService.LevelSkippedForAd();
                _gameFactory.DestroyPlayers();
                _stateMachine.Enter<LoadLevelState>();
            });
        }
    }
}
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Buttons
{
    public class ReplayGameButton : ButtonBase
    {
        [Inject] private StateMachine _stateMachine;
        [Inject] private AdsService _adsService;

        protected override void OnClick()
        {
            _adsService.PlayInterstitial();
            _stateMachine.Enter<ReplayLevelState>();
        }
    }
}
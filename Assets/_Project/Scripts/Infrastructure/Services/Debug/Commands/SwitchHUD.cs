using _Project.Scripts.Infrastructure.Services.Factories;

namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public class SwitchHUD : DebugCommand<bool>
    {
        public override string ID => "switch_hud";
        public override string Description => "Switch HUD";
        public override string Format => "switch_hud <bool>";

        private readonly UIFactory _uiFactory;

        public SwitchHUD(UIFactory uiFactory) => _uiFactory = uiFactory;

        public override void Invoke(bool state)
        {
            if (state)
                _uiFactory.GetHUD().Show();
            else
                _uiFactory.GetHUD().Hide();
        }
    }
}
using _Project.Scripts.Infrastructure.Services.Factories;

namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public class SwitchUI : DebugCommand<bool>
    {
        public override string ID => "switch_ui";
        public override string Description => "Switch UI";
        public override string Format => "switch_ui <bool>";

        private readonly UIFactory _uiFactory;

        public SwitchUI(UIFactory uiFactory) => _uiFactory = uiFactory;

        public override void Invoke(bool state) => _uiFactory.GetUIRoot().gameObject.SetActive(state);
    }
}
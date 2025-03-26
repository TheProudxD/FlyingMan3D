using _Project.Scripts.Infrastructure.DI;

namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public class SwitchDIBindingDebugCommand : DebugCommand<bool>
    {
        public override string ID => "switch_di_binding";
        public override string Description => "Switch di binding";
        public override string Format => "switch_di_binding <bool>";

        private const string PROJECTSCOPE_PATH = "ProjectScope";

        public override void Invoke(bool state) => UnityEngine.Resources.Load<ProjectInstaller>(PROJECTSCOPE_PATH).gameObject.SetActive(state);
    }
}
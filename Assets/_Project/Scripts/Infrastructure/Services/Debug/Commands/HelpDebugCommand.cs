namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public class HelpDebugCommand : DebugCommand
    {
        public override string ID => "help";
        public override string Description => "Shows help commands";
        public override string Format => "help";

        public bool ShowHelp { get; private set; }

        public override void Invoke()
        {
            ShowHelp = true;
        }
    }
}
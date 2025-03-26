namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public abstract class DebugCommandBase
    {
        public abstract string ID { get; }
        public abstract string Description { get; }
        public abstract string Format { get; }
    }
}
namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public abstract class DebugCommand : DebugCommandBase
    {
        public abstract void Invoke();
    }

    public abstract class DebugCommand<T> : DebugCommandBase
    {
        public abstract void Invoke(T value);
    }
}
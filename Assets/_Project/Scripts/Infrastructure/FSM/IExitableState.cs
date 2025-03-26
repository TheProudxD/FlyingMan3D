namespace _Project.Scripts.Infrastructure.FSM
{
    public interface IExitableState
    {
        void SetStateMachine(StateMachine value);
        void Exit();
    }
}
namespace _Project.Scripts.Infrastructure.FSM
{
    public interface IUpdatableState
    {
        void LogicUpdate();
        void PhysicsUpdate();
    }
}
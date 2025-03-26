namespace _Project.Scripts.Infrastructure.FSM
{
    public interface IPayLoadState<in TPayload> : IExitableState
    {
        public void Enter(TPayload payload);
    }
}
namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class ContinueLevelState : IState
    {
        private StateMachine _value;

        public ContinueLevelState() { }

        public void SetStateMachine(StateMachine value) => _value = value;

        public void Enter() { }

        public void Exit() { }
    }
}
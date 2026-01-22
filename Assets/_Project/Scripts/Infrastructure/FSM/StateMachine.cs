using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.FSM.States;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM
{
    public class StateMachine
    {
        public IExitableState CurrentState => _currentState;

        private readonly Dictionary<Type, IExitableState> _states = new();

        private IExitableState _currentState;

        public StateMachine(params IExitableState[] states)
        {
            RegisterStates(states);
        }
        
        public void RegisterStates(IEnumerable<IExitableState> states)
        {
            foreach (IExitableState state in states)
            {
                RegisterState(state);
            }
        }

        public void RegisterState(IExitableState state)
        {
            state.SetStateMachine(this);
            _states[state.GetType()] = state;
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
            state?.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayLoadState<TPayload>
        {
            var state = ChangeState<TState>();
            state?.Enter(payload);
        }

        public void Initialize() => Enter<BootstrapState>();

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _currentState?.Exit();

            var state = GetState<TState>();
            _currentState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            if (_states.TryGetValue(typeof(TState), out IExitableState newState) == false)
            {
                Debug.LogError($"{typeof(TState)} doesn't exist.");
            }

            return (TState)newState;
        }
    }
}
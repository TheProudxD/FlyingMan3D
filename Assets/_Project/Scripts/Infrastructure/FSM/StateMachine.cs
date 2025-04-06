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

        public StateMachine(BootstrapState bootstrapState, LoadLevelState loadLevelState,
            LoadGameState loadGameState, GameLoopState gameLoopState, WinLevelState winLevelState,
            LoseLevelState loseLevelState, RestartLevelState restartLevelState, ReplayLevelState replayLevelState,
            ContinueLevelState continueLevelState)
        {
            /*IExitableState[] states = { bootstrapState, loadLevelState, loadProgressState, gameLoopState, gameOverState };
            foreach (IExitableState state in states)
            {
                state.SetStateMachine(this);
                _states.Add(state.GetType(), state);
            }*/

            bootstrapState.SetStateMachine(this);
            _states.Add(bootstrapState.GetType(), bootstrapState);
            loadLevelState.SetStateMachine(this);
            _states.Add(loadLevelState.GetType(), loadLevelState);
            loadGameState.SetStateMachine(this);
            _states.Add(loadGameState.GetType(), loadGameState);
            gameLoopState.SetStateMachine(this);
            _states.Add(gameLoopState.GetType(), gameLoopState);
            winLevelState.SetStateMachine(this);
            _states.Add(winLevelState.GetType(), winLevelState);
            loseLevelState.SetStateMachine(this);
            _states.Add(loseLevelState.GetType(), loseLevelState);
            restartLevelState.SetStateMachine(this);
            _states.Add(restartLevelState.GetType(), restartLevelState);
            replayLevelState.SetStateMachine(this);
            _states.Add(replayLevelState.GetType(), replayLevelState);
            continueLevelState.SetStateMachine(this);
            _states.Add(continueLevelState.GetType(), continueLevelState);
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
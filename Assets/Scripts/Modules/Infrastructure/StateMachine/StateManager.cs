using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Modules.Infrastructure.States;
using UnityEngine;
/// <summary>
/// PlayerStateMachine, EnemyStateMachine, UIStateMachine
/// </summary>
/// <typeparam name="TState"></typeparam>
namespace Modules.Infrastructure.States
{
    public abstract class StateManager<TState> : MonoBehaviour where TState : Enum
    {
        protected Dictionary<TState, BaseState<TState>> _states = new Dictionary<TState, BaseState<TState>>();
        protected BaseState<TState> _currentState;
        protected bool _isTransitioningState;
        void Start() {
            _currentState.EnterState();
        }
        void Update() {
            TState nextStateKey = _currentState.GetNextState();
            if(nextStateKey.Equals(_currentState.StateKey)) 
            {
                _currentState.UpdateState();
            } else if(!_isTransitioningState)
            {
                TransitionToNextState(nextStateKey);

            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateKey"></param>
        private void TransitionToNextState(TState stateKey)
        {
            _isTransitioningState = true;
        if(_currentState is null) return;
        _currentState.ExitState();
        if(_states.TryGetValue(stateKey, out _currentState))
        {
            _currentState.EnterState();
        }
        _isTransitioningState = false;
        }

        void OnTriggerEnter(Collider other){
            _currentState.OnTriggerEnter(other);
        }
        void OnTriggerExit(Collider other){
            _currentState.OnTriggerExit(other);
        }
        void OnTriggerStay(Collider other) {
            _currentState.OnTriggerStay(other);
        }
    }
}

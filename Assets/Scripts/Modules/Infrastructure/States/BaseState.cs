using System;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public abstract class BaseState<TState> where TState : Enum
    {
        public BaseState(TState key)
        {
            StateKey = key;
        }
        public TState StateKey {
            get {return StateKey;}
            private set {StateKey = value;}
        }
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
        public abstract TState GetNextState();
        public abstract void OnTriggerEnter(Collider other);
        public abstract void OnTriggerStay(Collider other);
        public abstract void OnTriggerExit(Collider other);

    }
    
}

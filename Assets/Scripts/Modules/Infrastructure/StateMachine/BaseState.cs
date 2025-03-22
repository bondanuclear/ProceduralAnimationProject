using System;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public abstract class BaseState<TState> where TState : Enum
    {
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
        public abstract TState GetNextState();
        public abstract void OnTriggerEnter();
        public abstract void OnTriggerStay();
        public abstract void OnTriggerExit();

    }
    
}

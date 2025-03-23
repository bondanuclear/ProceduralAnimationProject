using UnityEngine;

namespace Modules.Infrastructure.States
{
    /// <summary>
    /// Shared interactions state logic. Спільна логіка для конкретного набору станів.
    /// This class will be inherited by concrete states of the state machine that we need. Each state will give an appropriate key of generic (enum type)
    /// </summary>
    public abstract class EnvironmentInteractionState : BaseState<EEnvironmentInteractionState>
    {
        protected EnvironmentInteractionContext _environmentInteractionContext;
        public EnvironmentInteractionState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(stateKey)
        {
            _environmentInteractionContext = environmentInteractionContext;
        }

        public override void EnterState()
        {
            
        }

        public override void ExitState()
        {
            
        }

        public override EEnvironmentInteractionState GetNextState()
        {
            return EEnvironmentInteractionState.Search;
        }

        public override void OnTriggerEnter(Collider other)
        {
            
        }

        public override void OnTriggerExit(Collider other)
        {
            
        }

        public override void OnTriggerStay(Collider other)
        {
           
        }

        public override void UpdateState()
        {
           
        }
    }
}
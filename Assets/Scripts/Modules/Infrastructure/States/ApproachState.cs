using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class ApproachState : EnvironmentInteractionState
    {
        public ApproachState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
        {
            
        }
        public override void EnterState()
        {
            
        }

        public override void ExitState()
        {
            
        }

        public override EEnvironmentInteractionState GetNextState()
        {
            return StateKey;
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
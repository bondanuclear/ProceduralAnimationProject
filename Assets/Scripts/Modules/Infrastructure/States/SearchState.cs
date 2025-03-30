using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class SearchState : EnvironmentInteractionState
    {
        private EEnvironmentInteractionState _stateKey;
        
        public SearchState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
        {
            _stateKey = stateKey;
           
        }

        public override void EnterState()
        {
            Debug.Log("Entered search state");
        }

        public override void ExitState()
        {
            Debug.Log("Exited search state");
        }
        public override void UpdateState()
        {
            //Debug.Log("Updating search state");
        }
        public override EEnvironmentInteractionState GetNextState()
        {
            return StateKey;
        }
        public override void OnTriggerEnter(Collider other)
        {
            //Debug.Log("On trigger enter search state: " + other.name);
            StartIKTargetPositionTracking(other);
        }
        override public void OnTriggerExit(Collider other)
        {
            ResetIKTargetPositionTracking(other);
        }
        public override void OnTriggerStay(Collider other)
        {
            UpdateIKTargetPositionTracking(other);
        }

        
    }
}

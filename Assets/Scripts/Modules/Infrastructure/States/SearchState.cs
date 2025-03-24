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
        public override EEnvironmentInteractionState GetNextState()
        {
            return StateKey;
        }
        public override void OnTriggerEnter(Collider other)
        {
            StartIKTargetPositionTracking(other);
        }
        override public void OnTriggerExit(Collider other)
        {
            ResetIKTargetPositionTracking(other);
        }
    }
}

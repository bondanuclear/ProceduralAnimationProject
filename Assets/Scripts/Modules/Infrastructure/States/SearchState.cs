using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class SearchState : EnvironmentInteractionState
    {
        private float _approachDistanceThreshold = 2f;
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
            bool isCloseToTarget = Vector3.Distance(_context.ClosestPointOnColliderFromShoulder, _context.RootTransform.position) < _approachDistanceThreshold;
            bool isClosestPointOnColliderValid = _context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;
            if(isCloseToTarget && isClosestPointOnColliderValid)
            {
                return EEnvironmentInteractionState.Approach;
            }
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

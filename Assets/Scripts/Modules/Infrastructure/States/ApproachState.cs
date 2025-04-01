using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class ApproachState : EnvironmentInteractionState
    {
        private float _elapsedTime = 0;
        private float _approachWeight = 0.5f;
        private float _lerpDuration = 5;
        public ApproachState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
        {
            
        }
        public override void EnterState()
        {

            Debug.Log("Entered Approach State");
            _elapsedTime = 0;
        }

        public override void ExitState()
        {
            
        }
        public override void UpdateState()
        {
            _elapsedTime += Time.deltaTime;
            _context.CurrentIKConstraint.weight = Mathf.Lerp(_context.CurrentIKConstraint.weight, _approachWeight, _elapsedTime / _lerpDuration);
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
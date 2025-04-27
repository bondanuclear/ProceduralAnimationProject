using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class ApproachState : EnvironmentInteractionState
    {
        private float _approachRotationWeight = .75f;
        private float _elapsedTime = 0;
        private float _approachWeight = 0.5f;
        private float _lerpDuration = 5;
        private float _rotationSpeed = 200;
        private float _armsReachDistance = 0.5f;
        private float _approachDuration = 2f;
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
            // create a quaternion with z-axis pointing down towards the ground
            Quaternion expectedRotation = Quaternion.LookRotation(-Vector3.up, _context.RootTransform.forward);
            _context.CurrentIKTargetTransform.rotation = Quaternion.RotateTowards(_context.CurrentIKTargetTransform.rotation, expectedRotation, Time.deltaTime * _rotationSpeed);
            _elapsedTime += Time.deltaTime;
            _context.CurrentIKConstraint.weight = Mathf.Lerp(_context.CurrentIKConstraint.weight, _approachWeight, _elapsedTime / _lerpDuration);
            _context.CurrentMultiRotationConstraint.weight = Mathf.Lerp(_context.CurrentMultiRotationConstraint.weight, _approachRotationWeight, _elapsedTime / _lerpDuration);
        }
        public override EEnvironmentInteractionState GetNextState()
        {
            bool isOverStateDuration = _elapsedTime >= _approachDuration;
            if(isOverStateDuration)
            {
                return EEnvironmentInteractionState.Reset;
            }
            bool isWithinArmsReach = Vector3.Distance(_context.ClosestPointOnColliderFromShoulder, _context.CurrentShoulderTransform.position) < _armsReachDistance;
            bool isClosestPointOnColliderFromShoulderValid = _context.ClosestPointOnColliderFromShoulder != Vector3.positiveInfinity;
            if(isWithinArmsReach && isClosestPointOnColliderFromShoulderValid)
            {
                return EEnvironmentInteractionState.Rise;
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
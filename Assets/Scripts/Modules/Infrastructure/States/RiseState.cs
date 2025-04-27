using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class RiseState : EnvironmentInteractionState
    {
        private float _elapsedTime = 0;
        private float _lerpDuration = 5f;
        private float _riseWeight = 1f;
        private float _riseRotationWeight = 1f;
        private LayerMask _interactionLayerMask = LayerMask.GetMask("Interactable");
        private Quaternion _expectedHandRotation;
        private float _rotationSpeed = 200f;
        public RiseState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
        {
        
        }

        public override void EnterState()
        {
            Debug.Log("Rise State Enter");
        }

        public override void ExitState()
        {
            Debug.Log("Rise State Exit");
        }

        public override EEnvironmentInteractionState GetNextState()
        {
            return StateKey;
        }

        public override void OnTriggerEnter(Collider other)
        {
            StartIKTargetPositionTracking(other);
        }

        public override void OnTriggerExit(Collider other)
        {
            ResetIKTargetPositionTracking(other);
        }

        public override void OnTriggerStay(Collider other)
        {
            UpdateIKTargetPositionTracking(other);
        }

        public override void UpdateState()
        {
            CalculateExpectedHandRotation();
            _elapsedTime += Time.deltaTime;
            _context.InteractionPointYOffset = Mathf.Lerp(_context.InteractionPointYOffset, _context.ClosestPointOnColliderFromShoulder.y, _elapsedTime / _lerpDuration);
            _context.CurrentIKConstraint.weight = Mathf.Lerp(_context.CurrentIKConstraint.weight, _riseWeight, _elapsedTime / _lerpDuration);
             _context.CurrentMultiRotationConstraint.weight = Mathf.Lerp(_context.CurrentMultiRotationConstraint.weight, _riseRotationWeight, _elapsedTime / _lerpDuration);
            _context.CurrentIKTargetTransform.rotation = Quaternion.RotateTowards(_context.CurrentIKTargetTransform.rotation, _expectedHandRotation, Time.deltaTime * _rotationSpeed);
            Debug.Log("Rise State Update");

        }
        private void CalculateExpectedHandRotation()
        {
            Vector3 startPos = _context.CurrentShoulderTransform.position;
            Vector3 endPos = _context.ClosestPointOnColliderFromShoulder;
            Vector3 direction = (endPos - startPos).normalized;
            if(Physics.Raycast(startPos, direction, out RaycastHit hit, .5f, _interactionLayerMask))
            {
                Debug.Log("Hit: " + hit.point);
                Vector3 surfaceNormal = hit.normal;
                Vector3 targetForward = -surfaceNormal;
                _expectedHandRotation = Quaternion.LookRotation(targetForward, Vector3.up);
            }
        }
    }
}

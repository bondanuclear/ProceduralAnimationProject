
using System.Collections;
using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States.MovementStates
{
    public class StopState : MovementState
    {
        private float _stateTimer = 0f;
        public StopState(MovementContext context, EMovementState stateKey) : base(context, stateKey)
        {
        }
        
        public override void EnterState()
        {
            Debug.Log("StopState EnterState");
            
            _stateTimer = 0f;
            //_context.SetStateParameters(EMovementState.Stop);
        }

        public override void ExitState()
        {
            _stateTimer = 0f;
            //_context.ShouldUpdateSpineTarget = false;
            Debug.Log("StopState ExitState");
        }

        private IEnumerator LerpSpineIKWeight(float startWeight, float endWeight, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                _context.SpineIK.weight = Mathf.Lerp(_context.SpineIK.weight, endWeight, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _context.SpineIK.weight = endWeight;
            _context.SpineTarget.localPosition = _context.SpineTargetOriginalPosition;
        }

        public override EMovementState GetNextState()
        {
            Debug.Log("StopState GetNextState");
            if (_context.CharacterController.velocity.magnitude == 0)
            {
                // _context.MonoBehaviour.StartCoroutine(LerpSpineIKWeight(1f, 0f, 0.5f));
                // Wait 2 seconds before transitioning to idle
                if (_stateTimer >= 2f)
                {
                    return EMovementState.Idle;
                }
                _stateTimer += Time.deltaTime;
            }

            if (_context.CharacterController.velocity.magnitude > 0)
            {
                _context.ShouldUpdateSpineTarget = false;
                return EMovementState.Walk;
            }
            
            return EMovementState.Stop;
        }
        
        public override void UpdateState()
        {
            Debug.Log("StopState UpdateState");
            //_context.SpineIK.weight = Mathf.Lerp(_context.SpineIK.weight, 1f, Time.deltaTime * 50f);
            //_context.SpineTarget.localPosition  = new Vector3(_context.SpineTarget.localPosition.x, _context.SpineTarget.localPosition.y, _context.TargetTransform.localPosition.z);
            // Stop state doesn't need special updates
        }
    }
} 
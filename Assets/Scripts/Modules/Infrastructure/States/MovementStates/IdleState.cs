// IdleState.cs

using System.Collections;
using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States.MovementStates
{
    public class IdleState : MovementState
    {
        public IdleState(MovementContext context, EMovementState stateKey) : base(context, stateKey)
        {
        }
        
        public override void EnterState()
        {
            Debug.Log("IdleState EnterState");
            _context.ShouldUpdateSpineTarget = false;
            if(_context.SpineIK.weight > 0f)
            {
                _context.MonoBehaviour.StartCoroutine(LerpSpineIKWeight(1f, 0f, 1.5f));
            } 
           
        }
        private IEnumerator LerpSpineIKWeight(float startWeight, float endWeight, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                _context.SpineIK.weight = Mathf.Lerp(startWeight, endWeight, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _context.SpineIK.weight = endWeight;
            _context.SpineTarget.localPosition = _context.SpineTargetOriginalPosition;
        }
        public override void ExitState()
        {
           Debug.Log("IdleState ExitState");
        }


        public override EMovementState GetNextState()
        {
            Vector3 horizontalVelocity = new Vector3(_context.CharacterController.velocity.x, 0, _context.CharacterController.velocity.z);
            
            if (horizontalVelocity.magnitude > 0.1f)
            {
                return EMovementState.Walk;
            }
            
            
            return EMovementState.Idle;
        }
        
        public override void UpdateState()
        {
            Debug.Log("IdleState UpdateState");
            
        }
    }
}   
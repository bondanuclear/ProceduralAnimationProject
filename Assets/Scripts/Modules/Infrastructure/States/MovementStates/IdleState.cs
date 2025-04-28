// IdleState.cs

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
            //_context.SetStateParameters(EMovementState.Idle);
        }

        public override void ExitState()
        {
           Debug.Log("IdleState ExitState");
        }

        private float _movementStartTime = -1f;

        public override EMovementState GetNextState()
        {
            Debug.Log("IdleState GetNextState");
            Debug.Log(_context.CharacterController.velocity.magnitude);
            // Only transition if horizontal velocity is significant and not just falling
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
            // Idle state doesn't need special updates
        }
    }
}   
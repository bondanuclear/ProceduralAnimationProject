using UnityEngine;

using Modules.Infrastructure.States;

namespace Modules.Infrastructure.States.MovementStates
{
    public class WalkState : MovementState
    {
        public WalkState(MovementContext context, EMovementState stateKey) : base(context, stateKey)
        {
        }
        
        public override void EnterState()
        {
            Debug.Log("WalkState EnterState");
            //_context.SetStateParameters(EMovementState.Walk);
        }

        public override void ExitState()
        {
            Debug.Log("WalkState ExitState");
            // No special cleanup needed when exiting walk state
        }
        
        public override EMovementState GetNextState()
        {
            //Debug.Log("WalkState GetNextState: " + _context.CharacterController.velocity.magnitude);
            Debug.Log("WalkState GetNextState: ");
            if (_context.CharacterController.velocity.magnitude == 0)
            {
                Debug.Log("WalkState GetNextState: Stop");
                return EMovementState.Stop;
            }
            else if (_context.CharacterController.velocity.magnitude >= 4)
            {
                Debug.Log("WalkState GetNextState: Run");
                return EMovementState.Run;
            }
            else
            {
                Debug.Log("WalkState GetNextState: Walk");
                return EMovementState.Walk;
            }
        }
        
        public override void UpdateState()
        {
            Debug.Log("WalkState UpdateState");
            // Walk state doesn't need special updates
        }
    }
} 
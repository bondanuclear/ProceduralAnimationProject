
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
            Debug.Log("StopState ExitState");
        }

        public override EMovementState GetNextState()
        {
            Debug.Log("StopState GetNextState");
            if (_context.CharacterController.velocity.magnitude == 0)
            {
                // Wait 2 seconds before transitioning to idle
                if (_stateTimer >= 2f)
                {
                    return EMovementState.Idle;
                }
                _stateTimer += Time.deltaTime;
            }

            if (_context.CharacterController.velocity.magnitude > 0)
            {
                return EMovementState.Walk;
            }
            
            return EMovementState.Stop;
        }
        
        public override void UpdateState()
        {
            Debug.Log("StopState UpdateState");
            // Stop state doesn't need special updates
        }
    }
} 
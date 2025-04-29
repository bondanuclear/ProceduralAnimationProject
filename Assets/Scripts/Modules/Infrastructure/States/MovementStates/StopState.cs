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
            
            
            // Enable spine target updates
            _context.ShouldUpdateSpineTarget = true;
            
            // Start with full IK weight
            _context.SpineIK.weight = 1f;
            
            _context.SetStateParameters(EMovementState.Stop);
        }

        public override void ExitState()
        {
            _stateTimer = 0f;
            _context.ShouldUpdateSpineTarget = false;
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
                    Debug.LogError("StopState GetNextState: Transitioning to Idle");
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
            // A zero quaternion (0,0,0,0) is invalid and will not work with Slerp
            // We need to use Quaternion.identity for the default rotation
            _context.SpineTarget.localRotation = Quaternion.Slerp(_context.SpineTarget.localRotation, Quaternion.identity, Time.deltaTime * 5);
            
        }
        
       
    }
} 
using UnityEngine;

using Modules.Infrastructure.States;

namespace Modules.Infrastructure.States.MovementStates
{
    public class RunState : MovementState
    {
        public RunState(MovementContext context, EMovementState stateKey) : base(context, stateKey)
        {
        }
        
        public override void EnterState()
        {
            //_context.SpineIK.weight = 1f;
            _context.ShouldUpdateSpineTarget = true;
            Debug.Log("RunState EnterState");
            _context.SetStateParameters(EMovementState.Run);
        }

        public override void ExitState()
        {
            Debug.Log("RunState ExitState");
            //_context.SpineIK.weight = 1f;
            // No special cleanup needed when exiting run state
        }
        
        public override EMovementState GetNextState()
        {
            if (_context.CharacterController.velocity.magnitude == 0)
            {
                return EMovementState.Stop;
            }
            else if (_context.CharacterController.velocity.magnitude < 4)
            {
                return EMovementState.Walk;
            }
            
            return EMovementState.Run;
        }
        
        public override void UpdateState()
        {
            //_context.SpineIK.transform.position = new Vector3(_context.SpineIK.transform.position.x, _context.SpineIK.transform.position.y, _context.TargetTransform.position.z);
            _context.SpineIK.weight = Mathf.Lerp(_context.SpineIK.weight, 1f, Time.deltaTime * 5f);
            Debug.Log("RunState UpdateState");
            // Run state doesn't need special updates
        }
    }
} 
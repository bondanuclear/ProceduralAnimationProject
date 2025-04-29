using System.Collections;
using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States.MovementStates
{
    public class StopState : MovementState
    {
        private float _stateTimer = 0f;
        private float _spineFollowDuration = 0.5f; // Duration for spine to follow target
        private float _spineFollowProgress = 0f;
        private bool _spineFollowStarted = false;
        
        public StopState(MovementContext context, EMovementState stateKey) : base(context, stateKey)
        {
        }
        
        public override void EnterState()
        {
            Debug.Log("StopState EnterState");
            _stateTimer = 0f;
            _spineFollowProgress = 0f;
            _spineFollowStarted = false;
            
            // Enable spine target updates
            _context.ShouldUpdateSpineTarget = true;
            
            // Start with full IK weight
            _context.SpineIK.weight = 1f;
            
            _context.SetStateParameters(EMovementState.Stop);
        }

        public override void ExitState()
        {
            _stateTimer = 0f;
            _spineFollowProgress = 0f;
            _spineFollowStarted = false;
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
                // _context.MonoBehaviour.StartCoroutine(LerpSpineIKWeight(1f, 0f, 1.5f));
                //_context.SpineIK.weight = Mathf.Lerp(_context.SpineIK.weight, 0, 2f);
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
            // Start the spine follow animation when we first enter the state
            // if (!_spineFollowStarted && _context.CharacterController.velocity.magnitude < 0.1f)
            // {
            //     _spineFollowStarted = true;
            // }
            
            // // If the spine follow animation is in progress, update the spine target
            // if (_spineFollowStarted && _spineFollowProgress < 1.0f)
            // {
            //     _spineFollowProgress += Time.deltaTime / _spineFollowDuration;
            //     _spineFollowProgress = Mathf.Clamp01(_spineFollowProgress);
                
            //     // Apply easing function for more natural movement
            //     float easedProgress = EaseOutQuad(_spineFollowProgress);
                
            //     // We don't need to update the spine target position here
            //     // The MovementStateMachine's Update method will handle that
            //     // We just need to make sure ShouldUpdateSpineTarget is true
            // }
        }
        
        // Easing function for more natural movement
        private float EaseOutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }
    }
} 
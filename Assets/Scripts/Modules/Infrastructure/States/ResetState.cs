using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class ResetState : EnvironmentInteractionState
    {
        private float _elapsedTime = 0;
        private float _resetDuration = 2f;
        public ResetState(EnvironmentInteractionContext context, EEnvironmentInteractionState stateKey) : base(context, stateKey)
        {
           
           
        }
        public override void OnTriggerEnter(Collider other)
        {
            
           
        }
        public override void OnTriggerStay(Collider other)
        {
            
           
        }
       
        public override void OnTriggerExit(Collider other)
        {
          
        }
        public override void EnterState()
        {
             Debug.Log("Entered Reset State");
           _elapsedTime = 0;
           _context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
           _context.CurrentIntersectingCollider = null;
        }
        public override void UpdateState()
        {
           _elapsedTime += Time.deltaTime;
        }
        public override EEnvironmentInteractionState GetNextState()
        {
            bool isMoving = _context.CharacterController.velocity != Vector3.zero;
            if( _elapsedTime >= _resetDuration && isMoving)
            {
                
                return EEnvironmentInteractionState.Search;
            }

            
            return StateKey;
        }

        public override void ExitState()
        {
            Debug.Log("Exiting reset state");
        }
    }
}
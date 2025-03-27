using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class ResetState : EnvironmentInteractionState
    {
        public ResetState(EnvironmentInteractionContext context, EEnvironmentInteractionState stateKey) : base(context, stateKey)
        {
            //Debug.Log("Called ResetState constructor!");
            EnvironmentInteractionContext _context = context;
        }
        public override void OnTriggerEnter(Collider other)
        {
            Debug.Log("Touched a wall in reset state " + other.name);
        }
        public override void OnTriggerStay(Collider other)
        {
            Debug.Log("Touching a wall in reset state " + other.name);
        }
        public override void OnTriggerExit(Collider other)
        {
            Debug.Log("No longer touching interactable " + other.name );
        }
        public override void EnterState()
        {
            Debug.Log("Entered Reset State");
        }
        public override void UpdateState()
        {
            Debug.Log("Updating reset state!");
        }
        public override EEnvironmentInteractionState GetNextState()
        {
            return StateKey;
        }

        public override void ExitState()
        {
            Debug.Log("Exiting reset state");
        }
    }
}
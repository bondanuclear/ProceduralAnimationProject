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
       
    }
}
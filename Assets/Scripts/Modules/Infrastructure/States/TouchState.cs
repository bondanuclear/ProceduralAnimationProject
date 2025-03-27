using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class TouchState : EnvironmentInteractionState
{
    private EEnvironmentInteractionState _stateKey;
    public TouchState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
    {
       //_stateKey = stateKey;
    }
    public override void EnterState()
        {
            
        }

        public override void ExitState()
        {
            
        }

        public override EEnvironmentInteractionState GetNextState()
        {
            return StateKey;
        }

        public override void OnTriggerEnter(Collider other)
        {
            
        }

        public override void OnTriggerExit(Collider other)
        {
            
        }

        public override void OnTriggerStay(Collider other)
        {
           
        }

        public override void UpdateState()
        {
            
        }
}
}

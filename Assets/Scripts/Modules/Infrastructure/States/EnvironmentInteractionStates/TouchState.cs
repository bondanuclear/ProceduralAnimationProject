using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class TouchState : EnvironmentInteractionState
    {
        private float _elapsedTime = 0;
        private float _touchDuration = .5f;
        public TouchState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
        {
        //_stateKey = stateKey;
        }
        public override void EnterState()
        {
            Debug.Log("Touch State Enter");
            _elapsedTime = 0;
        }

        public override void ExitState()
        {
            Debug.Log("Touch State Exit");
        }

        public override EEnvironmentInteractionState GetNextState()
        {
            if(_elapsedTime >= _touchDuration)
            {
                return EEnvironmentInteractionState.Reset;
            }
            return StateKey;
        }

        public override void OnTriggerEnter(Collider other)
        {
            StartIKTargetPositionTracking(other);
        }

        public override void OnTriggerExit(Collider other)
        {
            ResetIKTargetPositionTracking(other);
        }

        public override void OnTriggerStay(Collider other)
        {
            UpdateIKTargetPositionTracking(other);
        }

        public override void UpdateState()
        {
            Debug.Log("Touch State Update");
            _elapsedTime += Time.deltaTime;
            
        }
}
}

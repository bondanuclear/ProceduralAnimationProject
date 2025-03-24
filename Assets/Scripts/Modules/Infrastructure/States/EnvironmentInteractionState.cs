using UnityEngine;

namespace Modules.Infrastructure.States
{
    /// <summary>
    /// Shared interactions state logic. Спільна логіка для конкретного набору станів.
    /// This class will be inherited by concrete states of the state machine that we need. Each state will give an appropriate key of generic (enum type)
    /// </summary>
    public abstract class EnvironmentInteractionState : BaseState<EEnvironmentInteractionState>
    {
        private const string InteractableLayer = "Interactable";
        protected EnvironmentInteractionContext _environmentInteractionContext;
        public EnvironmentInteractionState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(stateKey)
        {
            _environmentInteractionContext = environmentInteractionContext;
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
        protected void StartIKTargetPositionTracking(Collider intersectingCollider)
        {
            if(intersectingCollider.gameObject.layer == LayerMask.NameToLayer(InteractableLayer) && _environmentInteractionContext.CurrentIntersectingCollider == null)
            {
                _environmentInteractionContext.CurrentIntersectingCollider = intersectingCollider;
                Vector3 closestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, _environmentInteractionContext.RootTransform.position);
                _environmentInteractionContext.SetCurrentSide(closestPointFromRoot);
            }
            
        }
        protected void ResetIKTargetPositionTracking(Collider intersectingCollider)
        {
            if(_environmentInteractionContext.CurrentIntersectingCollider  == intersectingCollider)
            {
                _environmentInteractionContext.CurrentIntersectingCollider = null;
            }
        }
        protected void UpdateIKTargetPositionTracking(Collider intersectingCollider)
        {
            
        }

        private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
        {
            return intersectingCollider.ClosestPoint(positionToCheck);
        }
    }
}
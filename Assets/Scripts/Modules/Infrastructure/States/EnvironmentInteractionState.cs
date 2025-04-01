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
        protected EnvironmentInteractionContext _context;
        public EnvironmentInteractionState(EnvironmentInteractionContext context, EEnvironmentInteractionState stateKey) : base(stateKey)
        {
            //Debug.Log("Called EnvironmentInteractionState constructor with ");
            _context ??= context;
           
        }

        protected void StartIKTargetPositionTracking(Collider intersectingCollider)
        {
            if(intersectingCollider.gameObject.layer == LayerMask.NameToLayer(InteractableLayer) && _context.CurrentIntersectingCollider == null)
            {
                
                _context.CurrentIntersectingCollider = intersectingCollider;
                
                Vector3 closestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, _context.RootTransform.position);
                _context.SetCurrentSide(closestPointFromRoot);
                SetIKTargetPosition();
            }
            
        }
        protected void ResetIKTargetPositionTracking(Collider intersectingCollider)
        {
            if(intersectingCollider == _context.CurrentIntersectingCollider)
            {
                _context.CurrentIntersectingCollider = null;
                _context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity; 
                //_context.CurrentIKConstraint.weight = 0;
            }
        }
        protected void UpdateIKTargetPositionTracking(Collider intersectingCollider)
        {
            if(_context.CurrentIntersectingCollider is null)
            {
                //Debug.Log("Stopping UpdateIKTargetPositionTracking");
                return;
            }
            if(intersectingCollider == _context.CurrentIntersectingCollider)
            {
                SetIKTargetPosition();
            }
        }

        private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
        {
            
            return intersectingCollider.ClosestPoint(positionToCheck);
        }
        private void SetIKTargetPosition()
        {
            if(_context is null) return;
            if(_context.CurrentIntersectingCollider is null)
            {
                Debug.Log("_context.CurrentIntersectingCollider is null");
                return;
            }
            
            _context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(_context.CurrentIntersectingCollider, 
               new Vector3(_context.CurrentShoulderTransform.position.x, _context.CharacterShoulderHeight, _context.CurrentShoulderTransform.position.z) );
            var directionRay = _context.CurrentShoulderTransform.position - _context.ClosestPointOnColliderFromShoulder;
            var normalizedDirection = directionRay.normalized;
            float offsetDistance = 0.05f;
            Vector3 offset = normalizedDirection * offsetDistance;
            Vector3 offsetPosition = _context.ClosestPointOnColliderFromShoulder + offset;
            _context.CurrentIKTargetTransform.position = offsetPosition;
            //_context.CurrentIKConstraint.weight = 1;
        }
    }
}
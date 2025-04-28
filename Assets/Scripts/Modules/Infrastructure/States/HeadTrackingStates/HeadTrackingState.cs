using Modules.Infrastructure.HeadTracking;
using UnityEngine;

namespace Modules.Infrastructure.States
{
    public abstract class HeadTrackingState : BaseState<EHeadTrackingState>
    {
        protected HeadTrackingContext _context;

        public HeadTrackingState(HeadTrackingContext context, EHeadTrackingState stateKey) : base(stateKey)
        {
            _context = context;
        }

        public override void OnTriggerEnter(Collider other)
        {
            // Optional: Override in derived classes if needed
        }

        public override void OnTriggerExit(Collider other)
        {
            // Optional: Override in derived classes if needed
        }

        public override void OnTriggerStay(Collider other)
        {
            // Optional: Override in derived classes if needed
        }

        protected bool IsInFieldOfView(Vector3 targetPosition)
        {
            Vector3 directionToTarget = targetPosition - _context.HeadTransform.position;
            float angle = Vector3.Angle(_context.HeadTransform.forward, directionToTarget);
            return angle <= _context.MaxTrackingAngle;
        }

        protected GameObject FindClosestTrackableObject()
        {
            Collider[] colliders = Physics.OverlapSphere(
                _context.HeadTransform.position, 
                _context.DetectionRadius, 
                _context.TrackableLayerMask
            );

            GameObject closestObject = null;
            float closestDistance = float.MaxValue;
            
            foreach (Collider collider in colliders)
            {
                // Don't track self
                if (collider.transform.IsChildOf(_context.HeadTransform.root))
                    continue;
                
                float distance = Vector3.Distance(_context.HeadTransform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = collider.gameObject;
                }
            }
            
            return closestObject;
        }
    }
} 
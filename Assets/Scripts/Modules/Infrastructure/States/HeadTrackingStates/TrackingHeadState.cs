using Modules.Infrastructure.HeadTracking;
using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class TrackingHeadState : HeadTrackingState
    {
        private const float MIN_TRACKING_DISTANCE = 0.5f;

        public TrackingHeadState(HeadTrackingContext context, EHeadTrackingState stateKey) : base(context, stateKey)
        {
        }

        public override void EnterState()
        {
            // Start with current weight
            _context.TrackingWeight = _context.HeadConstraint.weight;
        }

        public override void ExitState()
        {
            // Nothing special to do
        }

        public override EHeadTrackingState GetNextState()
        {
            // If target is null or destroyed, transition to returning state
            if (_context.CurrentTrackingTarget == null)
            {
                return EHeadTrackingState.Returning;
            }

            // If the target is too far or not in field of view anymore
            Vector3 targetPosition = _context.CurrentTrackingTarget.transform.position;
            float distance = Vector3.Distance(_context.HeadTransform.position, targetPosition);
            
            if (distance > _context.DetectionRadius || !IsInFieldOfView(targetPosition))
            {
                _context.CurrentTrackingTarget = null;
                return EHeadTrackingState.Returning;
            }

            return EHeadTrackingState.Tracking;
        }

        public override void UpdateState()
        {
            if (_context.CurrentTrackingTarget != null)
            {
                // Update target transform position to point at the tracked object
                _context.TargetTransform.position = _context.CurrentTrackingTarget.transform.position;
                
                // Smoothly increase the constraint weight for natural movement
                _context.TrackingWeight = Mathf.Lerp(_context.TrackingWeight, 1f, Time.deltaTime * _context.TrackingSpeed);
                _context.HeadConstraint.weight = _context.TrackingWeight;
            }
            else
            {
                // If somehow the target is null (destroyed during this frame)
                _context.CurrentTrackingTarget = FindClosestTrackableObject();
                if (_context.CurrentTrackingTarget == null)
                {
                    // If still no target found, we'll transition in the next frame
                }
            }
        }
    }
} 
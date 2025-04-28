using Modules.Infrastructure.HeadTracking;
using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class ReturningHeadState : HeadTrackingState
    {
        public ReturningHeadState(HeadTrackingContext context, EHeadTrackingState stateKey) : base(context, stateKey)
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
            // If we found a new tracking target while returning
            if (_context.CurrentTrackingTarget != null)
            {
                return EHeadTrackingState.Tracking;
            }

            // When weight is effectively zero, go to idle
            if (_context.HeadConstraint.weight < 0.01f)
            {
                return EHeadTrackingState.Idle;
            }

            return EHeadTrackingState.Returning;
        }

        public override void UpdateState()
        {
            // Smoothly decrease the constraint weight to return to original position
            _context.TrackingWeight = Mathf.Lerp(_context.TrackingWeight, 0f, Time.deltaTime * _context.ReturnSpeed);
            _context.HeadConstraint.weight = _context.TrackingWeight;

            // Periodically check if there's a new target while returning
            if (Random.value < 0.05f) // 5% chance each frame
            {
                _context.CurrentTrackingTarget = FindClosestTrackableObject();
            }
        }
    }
} 
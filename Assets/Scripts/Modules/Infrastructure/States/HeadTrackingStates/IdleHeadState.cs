using Modules.Infrastructure.HeadTracking;
using UnityEngine;

namespace Modules.Infrastructure.States
{
    public class IdleHeadState : HeadTrackingState
    {
        private float _searchTimer = 0f;
        private const float SEARCH_INTERVAL = 0.5f;

        public IdleHeadState(HeadTrackingContext context, EHeadTrackingState stateKey) : base(context, stateKey)
        {
        }

        public override void EnterState()
        {
            // Reset the head position to the original rotation
            _context.HeadConstraint.weight = 0f;
            _context.CurrentTrackingTarget = null;
            _searchTimer = 0f;
        }

        public override void ExitState()
        {
            // Nothing special to do
        }

        public override EHeadTrackingState GetNextState()
        {
            // If there's a trackable object, switch to tracking state
            if (_context.CurrentTrackingTarget != null)
            {
                return EHeadTrackingState.Tracking;
            }

            return EHeadTrackingState.Idle;
        }

        public override void UpdateState()
        {
            // Periodically search for trackable objects
            _searchTimer += Time.deltaTime;
            if (_searchTimer >= SEARCH_INTERVAL)
            {
                _searchTimer = 0f;
                _context.CurrentTrackingTarget = FindClosestTrackableObject();
            }
        }
    }
} 
using Modules.Infrastructure.HeadTracking;
using Modules.Infrastructure.States;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

namespace Modules.Infrastructure.StateMachine
{
    public class HeadTrackingStateMachine : StateManager<EHeadTrackingState>
    {
        [Header("Animation Rigging")]
        [SerializeField] private MultiAimConstraint _headConstraint;
        [SerializeField] private Transform _headBone;
        
        [Header("Tracking Settings")]
        [SerializeField] private float _maxTrackingAngle = 60f;
        [SerializeField] private float _trackingSpeed = 2f;
        [SerializeField] private float _returnSpeed = 1f;
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private string _trackableLayerName = "Trackable";
        
        private HeadTrackingContext _context;
        
        private void Awake()
        {
            ValidateComponents();
            
            _context = new HeadTrackingContext(
                _headConstraint,
                _headBone,
                _maxTrackingAngle,
                _trackingSpeed,
                _returnSpeed,
                _detectionRadius,
                _trackableLayerName
            );
            
            InitializeStates();
        }
        
        private void ValidateComponents()
        {
            Assert.IsNotNull(_headConstraint, "Head constraint is not assigned");
            Assert.IsNotNull(_headBone, "Head bone is not assigned");
        }
        
        private void InitializeStates()
        {
            // Add states to the state manager
            _states.Add(EHeadTrackingState.Idle, new IdleHeadState(_context, EHeadTrackingState.Idle));
            _states.Add(EHeadTrackingState.Tracking, new TrackingHeadState(_context, EHeadTrackingState.Tracking));
            _states.Add(EHeadTrackingState.Returning, new ReturningHeadState(_context, EHeadTrackingState.Returning));
            
            // Set initial state
            _currentState = _states[EHeadTrackingState.Idle];
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_headBone != null)
            {
                // Draw detection sphere
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_headBone.position, _detectionRadius);
                
                // Draw view cone
                if (_maxTrackingAngle > 0)
                {
                    Gizmos.color = Color.cyan;
                    Vector3 forward = _headBone.forward;
                    float coneLength = _detectionRadius * 0.8f;
                    
                    // Draw cone axis
                    Gizmos.DrawRay(_headBone.position, forward * coneLength);
                    
                    // Draw cone edges
                    float radianAngle = _maxTrackingAngle * Mathf.Deg2Rad;
                    float coneRadius = Mathf.Sin(radianAngle) * coneLength;
                    Vector3 coneEnd = _headBone.position + forward * coneLength;
                    
                    for (int i = 0; i < 8; i++)
                    {
                        float angle = i * Mathf.PI / 4f;
                        Vector3 dir = new Vector3(Mathf.Cos(angle) * coneRadius, Mathf.Sin(angle) * coneRadius, 0);
                        Vector3 worldDir = _headBone.TransformDirection(dir);
                        Vector3 point = coneEnd + worldDir;
                        Gizmos.DrawLine(_headBone.position, point);
                    }
                }
                
                // Draw target if available
                if (_context != null && _context.CurrentTrackingTarget != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(_headBone.position, _context.CurrentTrackingTarget.transform.position);
                    Gizmos.DrawWireSphere(_context.CurrentTrackingTarget.transform.position, 0.2f);
                }
            }
        }
    }
} 
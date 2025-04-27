
using Modules.Infrastructure.States;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

namespace Modules.Infrastructure.StateMachine
{
    public class EnvironmentInteractionStateMachine : StateManager<EEnvironmentInteractionState>
    {
        [Header("Animation Rigging")]
        [SerializeField] private TwoBoneIKConstraint _leftIKConstraint;
        [SerializeField] private TwoBoneIKConstraint _rightIKConstraint;
        [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
        [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;

        [Header("Physics")]
        //[SerializeField] private Rigidbody _rigidbody;
        //[SerializeField] private CapsuleCollider _collider;
        [SerializeField] private CharacterController _characterController;
        private EnvironmentInteractionContext _context;
        private void Awake() {
            ValidateConstraints();
            _context = new EnvironmentInteractionContext(_leftIKConstraint, 
                        _rightIKConstraint,_leftMultiRotationConstraint,_rightMultiRotationConstraint, transform.root, _characterController);
            InitializeStates();
            ConstructDetectionCollider();
        }
        private void ValidateConstraints()
        {
            Assert.IsNotNull(_leftIKConstraint, "Left IK constraint is not assigned");
            Assert.IsNotNull(_rightIKConstraint, "Right IK constraint is not assigned");
            Assert.IsNotNull(_leftMultiRotationConstraint, "Left MultiRotational constraint is not assigned");
            Assert.IsNotNull(_rightMultiRotationConstraint, "Right MultiRotational constraint is not assigned");
        } 
        private void InitializeStates()
        {
            // add states to inherited State Manager "states" dictionary and set initial state
            //Debug.Log("Called InitializeStates!");
            _states.Add(EEnvironmentInteractionState.Reset, new ResetState(_context, EEnvironmentInteractionState.Reset));
            _states.Add(EEnvironmentInteractionState.Search, new SearchState(_context, EEnvironmentInteractionState.Search));
            _states.Add(EEnvironmentInteractionState.Approach, new ApproachState(_context, EEnvironmentInteractionState.Approach));
            _states.Add(EEnvironmentInteractionState.Rise, new RiseState(_context, EEnvironmentInteractionState.Rise));
            _states.Add(EEnvironmentInteractionState.Touch, new TouchState(_context, EEnvironmentInteractionState.Touch));
            _currentState = _states[EEnvironmentInteractionState.Reset];
        }
        private void ConstructDetectionCollider()
        {
           
            float wingspan = _characterController.height;
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(wingspan, wingspan, wingspan);   
            boxCollider.center = new Vector3(_characterController.center.x, _characterController.center.y + (wingspan*0.25f), _characterController.center.z + (wingspan*0.5f));
            boxCollider.isTrigger = true;
            _context.ColliderCenterY = _characterController.center.y;
            
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if(_context != null && _context.ClosestPointOnColliderFromShoulder != null)
            {
                Gizmos.DrawSphere(_context.ClosestPointOnColliderFromShoulder,.03f);
            }
        }
    }
}

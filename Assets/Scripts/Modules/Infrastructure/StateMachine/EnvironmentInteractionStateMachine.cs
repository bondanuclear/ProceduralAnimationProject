
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;
namespace Modules.Infrastructure.States
{
    public class EnvironmentInteractionStateMachine : StateManager<EEnvironmentInteractionState>
    {
        [Header("Animation Rigging")]
        [SerializeField] private TwoBoneIKConstraint _leftIKConstraint;
        [SerializeField] private TwoBoneIKConstraint _rightIKConstraint;
        [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
        [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;

        [Header("Physics")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;

        private void Awake() {
            ValidateConstraints();
        }
        private void ValidateConstraints()
        {
            Assert.IsNotNull(_leftIKConstraint, "Left IK constraint is not assigned");
            Assert.IsNotNull(_rightIKConstraint, "Right IK constraint is not assigned");
            Assert.IsNotNull(_leftMultiRotationConstraint, "Left MultiRotational constraint is not assigned");
            Assert.IsNotNull(_rightMultiRotationConstraint, "Right MultiRotational constraint is not assigned");
        } 
    }
}

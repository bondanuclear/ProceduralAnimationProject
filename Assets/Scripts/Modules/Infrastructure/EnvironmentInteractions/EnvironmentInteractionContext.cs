using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    private TwoBoneIKConstraint _leftIKConstraint;
    private TwoBoneIKConstraint _rightIKConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private MultiRotationConstraint _rightMultiRotationConstraint;
    private Rigidbody _rigidbody;
    private Collider _collider;

    public EnvironmentInteractionContext(TwoBoneIKConstraint leftIKConstraint, TwoBoneIKConstraint rightIKConstraint, MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotationConstraint, Rigidbody rigidbody, Collider collider)
    {
        LeftIKConstraint = leftIKConstraint;
        RightIKConstraint = rightIKConstraint;
        LeftMultiRotationConstraint = leftMultiRotationConstraint;
        RightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _collider = collider;
    }

    public TwoBoneIKConstraint LeftIKConstraint { get => _leftIKConstraint; set => _leftIKConstraint = value; }
    public TwoBoneIKConstraint RightIKConstraint { get => _rightIKConstraint; set => _rightIKConstraint = value; }
    public MultiRotationConstraint LeftMultiRotationConstraint { get => _leftMultiRotationConstraint; set => _leftMultiRotationConstraint = value; }
    public MultiRotationConstraint RightMultiRotationConstraint { get => _rightMultiRotationConstraint; set => _rightMultiRotationConstraint = value; }
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }
    public Collider Collider { get => _collider; set => _collider = value; }
}
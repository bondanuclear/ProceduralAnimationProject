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
    private Transform _rootTransform;
    public EnvironmentInteractionContext(TwoBoneIKConstraint leftIKConstraint, TwoBoneIKConstraint rightIKConstraint, MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotationConstraint, Rigidbody rigidbody, Collider collider, Transform rootTransform)
    {
        LeftIKConstraint = leftIKConstraint;
        RightIKConstraint = rightIKConstraint;
        LeftMultiRotationConstraint = leftMultiRotationConstraint;
        RightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _collider = collider;
        _rootTransform = rootTransform;
    }

    public TwoBoneIKConstraint LeftIKConstraint { get => _leftIKConstraint; set => _leftIKConstraint = value; }
    public TwoBoneIKConstraint RightIKConstraint { get => _rightIKConstraint; set => _rightIKConstraint = value; }
    public MultiRotationConstraint LeftMultiRotationConstraint { get => _leftMultiRotationConstraint; set => _leftMultiRotationConstraint = value; }
    public MultiRotationConstraint RightMultiRotationConstraint { get => _rightMultiRotationConstraint; set => _rightMultiRotationConstraint = value; }
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }
    public Collider Collider { get => _collider; set => _collider = value; }
    public Transform RootTransform { get => _rootTransform; set => _rootTransform = value; }
    public Collider CurrentIntersectingCollider { get; set;}
    public TwoBoneIKConstraint CurrentIKConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIKTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }
   

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIKConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightIKConstraint.data.root.transform.position;

        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);
        if(isLeftCloser)
        {
            CurrentBodySide = EBodySide.Left;
            CurrentIKConstraint = _leftIKConstraint;
            CurrentMultiRotationConstraint = _leftMultiRotationConstraint;
        }
        else
        {
            CurrentBodySide = EBodySide.Right;
            CurrentIKConstraint = _rightIKConstraint;
            CurrentMultiRotationConstraint = _rightMultiRotationConstraint;
        }

        CurrentShoulderTransform = CurrentIKConstraint.data.root.transform;
        CurrentIKTargetTransform = CurrentIKConstraint.data.target.transform;
    }
}

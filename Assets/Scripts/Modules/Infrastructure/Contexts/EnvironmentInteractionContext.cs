using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    private TwoBoneIKConstraint _leftIKConstraint;
    private TwoBoneIKConstraint _rightIKConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private MultiRotationConstraint _rightMultiRotationConstraint;
    //private Rigidbody _rigidbody;
    //private Collider _collider;
    private Transform _rootTransform;
    private Vector3 _leftOriginalTargetPosition;
    private Vector3 _rightOriginalTargetPosition;
    private CharacterController _characterController;
    public EnvironmentInteractionContext(TwoBoneIKConstraint leftIKConstraint, TwoBoneIKConstraint rightIKConstraint, MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotationConstraint, Transform rootTransform, CharacterController characterController)
    {
        //Debug.Log("Called EnvironmentInteractionContext constructor!");
        _leftIKConstraint = leftIKConstraint;
        _rightIKConstraint = rightIKConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        // _rigidbody = rigidbody;
        // _collider = collider;
        _rootTransform = rootTransform;
        _leftOriginalTargetPosition = leftIKConstraint.data.target.localPosition;
        _rightOriginalTargetPosition = rightIKConstraint.data.target.localPosition;
        _characterController = characterController;
        CharacterShoulderHeight = leftIKConstraint.data.root.position.y;
        OriginalTargetRotation = leftIKConstraint.data.target.localRotation;
        SetCurrentSide(Vector3.positiveInfinity);
    }

    public TwoBoneIKConstraint LeftIKConstraint { get => _leftIKConstraint; set => _leftIKConstraint = value; }
    public TwoBoneIKConstraint RightIKConstraint { get => _rightIKConstraint; set => _rightIKConstraint = value; }
    public MultiRotationConstraint LeftMultiRotationConstraint { get => _leftMultiRotationConstraint; set => _leftMultiRotationConstraint = value; }
    public MultiRotationConstraint RightMultiRotationConstraint { get => _rightMultiRotationConstraint; set => _rightMultiRotationConstraint = value; }
    // public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }
    // public Collider Collider { get => _collider; set => _collider = value; }
    public Transform RootTransform { get => _rootTransform; set => _rootTransform = value; }
    public Collider CurrentIntersectingCollider { get; set;}
    public TwoBoneIKConstraint CurrentIKConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIKTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity;
    public float CharacterShoulderHeight { get; set; }
    public CharacterController CharacterController { get => _characterController; set => _characterController = value; }
    public float InteractionPointYOffset { get; set; } = 0f;
    public float ColliderCenterY { get; set; }
    public Vector3 CurrentOriginalTargetPosition { get; private set; }
    public Quaternion OriginalTargetRotation { get; private set; }

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIKConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightIKConstraint.data.root.transform.position;

        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);
        if(isLeftCloser)
        {
            Debug.Log("Left side is closer");
            CurrentBodySide = EBodySide.Left;
            CurrentIKConstraint = _leftIKConstraint;
            CurrentMultiRotationConstraint = _leftMultiRotationConstraint;
            CurrentOriginalTargetPosition = _leftOriginalTargetPosition;
            
        }
        else
        {
            Debug.Log("Right side is closer");
            CurrentBodySide = EBodySide.Right;
            CurrentIKConstraint = _rightIKConstraint;
            CurrentMultiRotationConstraint = _rightMultiRotationConstraint;
            CurrentOriginalTargetPosition = _rightOriginalTargetPosition;
            
        }

        CurrentShoulderTransform = CurrentIKConstraint.data.root.transform;
        CurrentIKTargetTransform = CurrentIKConstraint.data.target.transform;
        
    }
}

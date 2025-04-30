// MovementContext.cs
using Modules.Maths;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MovementContext
{
    private IEquationSolver _equationSolver;
    private Transform _characterTransform;
    private Transform _targetTransform;
    private CharacterController _characterController;
    private Transform _spineTarget;
    private TwoBoneIKConstraint _spineIK;
    private Vector3 _spineTargetOriginalPosition;
    // Movement parameters
    private float _currentSpeed;
    private float _walkSpeed;
    private float _runSpeed;
    private Vector3 _moveDirection;
    private MonoBehaviour _monoBehaviour;

    
    // Second-order dynamics parameters for different states
    private (float f, float z, float r) _walkParams;
    private (float f, float z, float r) _runParams;
   
    
    // Current state parameters
    private (float f, float z, float r) _currentParams;
    
    public MovementContext(Transform characterTransform, CharacterController characterController, 
                          float walkSpeed, float runSpeed,
                          (float f, float z, float r) walkParams,
                          (float f, float z, float r) runParams,
                          Transform spineTarget, TwoBoneIKConstraint spineIK, MonoBehaviour monoBehaviour, IEquationSolver equationSolver)
    {
        _characterTransform = characterTransform;
        _characterController = characterController;
        _walkSpeed = walkSpeed;
        _runSpeed = runSpeed;
        _spineTarget = spineTarget;
        _walkParams = walkParams;
        _runParams = runParams;
        _equationSolver = equationSolver;
        Debug.LogError($"_equationSolver: {_equationSolver.GetType().Name}");
        _spineIK = spineIK;
        _monoBehaviour = monoBehaviour;
        _spineTargetOriginalPosition = spineTarget.transform.localPosition;
        // Create target transform
        CreateMovementTarget();

        // Initialize with run parameters
        SetSecondOrderParameters();
    }

    private void SetSecondOrderParameters()
    {
        _currentParams = _runParams;
        Debug.Log("SetSecondOrderParameters: " + _currentParams.f + " " + _currentParams.z + " " + _currentParams.r);
        
    }

    private void CreateMovementTarget()
    {
        GameObject targetObj = new GameObject("MovementTarget");
        _targetTransform = targetObj.transform;
        _targetTransform.SetParent(_characterController.transform);
        _targetTransform.position = _characterController.transform.position;
    }

    // Update character position using equation solver
    public void UpdateTargetPosition()
    {
        _targetTransform.position = _equationSolver.UpdateValues(_characterController.transform.position, null, Time.fixedDeltaTime);
    }
    
    // Change dynamics parameters based on state
    // не використовується зараз
    public void SetStateParameters(EMovementState state)
    {
        switch (state)
        {
            case EMovementState.Walk:
                _currentParams = _walkParams;
                break;
            case EMovementState.Run:
                _currentParams = _runParams;
                break;
        }
        
        // Create new equation solver with new parameters
        // перезаписуємо значення параметрів
        //_equationSolver = new SemiImplicitEuler(_currentParams.f, _currentParams.z, _currentParams.r, _characterTransform.position);
    }
    
    // Properties
        public float CurrentSpeed { get => _currentSpeed; }
        public float WalkSpeed { get => _walkSpeed; }
        public float RunSpeed { get => _runSpeed; }
        public Vector3 MoveDirection { get => _moveDirection; }
        public Transform CharacterTransform { get => _characterTransform; }
        public Transform TargetTransform { get => _targetTransform; }
        public CharacterController CharacterController { get => _characterController; }
        public Transform SpineTarget { get => _spineTarget; private set => _spineTarget = value; } 
        public TwoBoneIKConstraint SpineIK { get => _spineIK; private set => _spineIK = value; } 
        public MonoBehaviour MonoBehaviour { get => _monoBehaviour; }
        public Vector3 SpineTargetOriginalPosition { get => _spineTargetOriginalPosition; private set => _spineTargetOriginalPosition = value; }
        public bool ShouldUpdateSpineTarget { get; set; }
}

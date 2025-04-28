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
    private (float f, float z, float r) _idleParams;
    private (float f, float z, float r) _walkParams;
    private (float f, float z, float r) _runParams;
    private (float f, float z, float r) _stopParams;
    
    // Current state parameters
    private (float f, float z, float r) _currentParams;
    
    public MovementContext(Transform characterTransform, CharacterController characterController, 
                          float walkSpeed, float runSpeed,
                          (float f, float z, float r) idleParams,
                          (float f, float z, float r) walkParams,
                          (float f, float z, float r) runParams,
                          (float f, float z, float r) stopParams, Transform spineTarget, TwoBoneIKConstraint spineIK, MonoBehaviour monoBehaviour)
    {
        _characterTransform = characterTransform;
        _characterController = characterController;
        _walkSpeed = walkSpeed;
        _runSpeed = runSpeed;
        _spineTarget = spineTarget;
        _idleParams = idleParams;
        _walkParams = walkParams;
        _runParams = runParams;
        _stopParams = stopParams;
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
        _equationSolver = new SemiImplicitEuler(_currentParams.f, _currentParams.z, _currentParams.r, _characterTransform.position);
    }

    private void CreateMovementTarget()
    {
        GameObject targetObj = new GameObject("MovementTarget");
        _targetTransform = targetObj.transform;
        _targetTransform.SetParent(_characterController.transform);
        _targetTransform.position = _characterController.transform.position;
    }

    // Update movement based on input
    public void UpdateMovement(Vector3 inputDirection, bool isRunning)
    {
        _moveDirection = inputDirection.normalized;
        
        // Determine speed based on input and running state
        if (_moveDirection.magnitude > 0.1f)
        {
            _currentSpeed = isRunning ? _runSpeed : _walkSpeed;
        }
        else
        {
            _currentSpeed = 0f;
        }
        
        // Update target position
        if (_currentSpeed > 0)
        {
            _targetTransform.position += _moveDirection * _currentSpeed * Time.deltaTime;
        }
    }
    
    // Update character position using equation solver
    public void UpdateTargetPosition()
    {
        _targetTransform.position = _equationSolver.UpdateValues(_characterController.transform.position, null, Time.fixedDeltaTime);
    }
    
    // Change dynamics parameters based on state
    public void SetStateParameters(EMovementState state)
    {
        switch (state)
        {
            case EMovementState.Idle:
                _currentParams = _idleParams;
                break;
            case EMovementState.Walk:
                _currentParams = _walkParams;
                break;
            case EMovementState.Run:
                _currentParams = _runParams;
                break;
            case EMovementState.Stop:
                _currentParams = _stopParams;
                break;
        }
        
        // Create new equation solver with new parameters
        _equationSolver = new SemiImplicitEuler(_currentParams.f, _currentParams.z, _currentParams.r, _characterTransform.position);
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

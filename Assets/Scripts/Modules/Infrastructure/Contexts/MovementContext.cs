// MovementContext.cs
using Modules.Maths;
using UnityEngine;

public class MovementContext
{
    private IEquationSolver _equationSolver;
    private Transform _characterTransform;
    private Transform _targetTransform;
    private CharacterController _characterController;
    
    // Movement parameters
    private float _currentSpeed;
    private float _walkSpeed;
    private float _runSpeed;
    private Vector3 _moveDirection;
    
    // Second-order dynamics parameters for different states
    private (float f, float z, float r) _idleParams;
    private (float f, float z, float r) _walkParams;
    private (float f, float z, float r) _runParams;
    private (float f, float z, float r) _stopParams;
    
    // Current state parameters
    private (float f, float z, float r) _currentParams;
    
    // Constructor
    public MovementContext(Transform characterTransform, CharacterController characterController, 
                          float walkSpeed, float runSpeed,
                          (float f, float z, float r) idleParams,
                          (float f, float z, float r) walkParams,
                          (float f, float z, float r) runParams,
                          (float f, float z, float r) stopParams)
    {
        _characterTransform = characterTransform;
        _characterController = characterController;
        _walkSpeed = walkSpeed;
        _runSpeed = runSpeed;
        
        _idleParams = idleParams;
        _walkParams = walkParams;
        _runParams = runParams;
        _stopParams = stopParams;
        
        // Create target transform
        GameObject targetObj = new GameObject("MovementTarget");
        
        _targetTransform = targetObj.transform;
        _targetTransform.SetParent(_characterTransform.parent);
        _targetTransform.position = _characterTransform.position;
        
        // Initialize with idle parameters
        _currentParams = _runParams;
        _equationSolver = new SemiImplicitEuler(_currentParams.f, _currentParams.z, _currentParams.r, _characterTransform.position);
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
}

// MovementStateMachine.cs
using System.Collections;
using Modules.Infrastructure.States;
using Modules.Infrastructure.States.MovementStates;
using UnityEngine;
using UnityEngine.Animations.Rigging;
// MovementStateMachine.cs
public class MovementStateMachine : StateManager<EMovementState>
{
    [Header("Movement Settings")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _runThreshold = 0.7f;
    
    [Header("Second Order Parameters - Idle")]
    [SerializeField] private float _idleF = 0.5f;
    [SerializeField] private float _idleZ = 0.5f;
    [SerializeField] private float _idleR = 0.2f;
    
    [Header("Second Order Parameters - Walk")]
    [SerializeField] private float _walkF = 1f;
    [SerializeField] private float _walkZ = 0.7f;
    [SerializeField] private float _walkR = 0.5f;
    
    [Header("Second Order Parameters - Run")]
    [SerializeField] private float _runF = 1.5f;
    [SerializeField] private float _runZ = 0.8f;
    [SerializeField] private float _runR = 0.6f;
    
    [Header("Second Order Parameters - Stop")]
    [SerializeField] private float _stopF = 0.8f;
    [SerializeField] private float _stopZ = 0.9f;
    [SerializeField] private float _stopR = 0.7f;
    [Header("Spine Target")]
    [SerializeField] private Transform _spineTarget;
    [SerializeField] private TwoBoneIKConstraint _spineIK;
    private MovementContext _context;
    private Vector3 _inputDirection;
    private bool _isRunning;
    
    private void Awake()
    {
        // Create context with parameters
        _context = new MovementContext(
            transform,
            _characterController,
            _walkSpeed,
            _runSpeed,
            (_idleF, _idleZ, _idleR),
            (_walkF, _walkZ, _walkR),
            (_runF, _runZ, _runR),
            (_stopF, _stopZ, _stopR),
            _spineTarget,
            _spineIK,
            this
        );
        
        // Initialize states
        InitializeStates();
        
    }
    
    private void InitializeStates()
    {
        _states.Add(EMovementState.Idle, new IdleState(_context, EMovementState.Idle));
        _states.Add(EMovementState.Walk, new WalkState(_context, EMovementState.Walk));
        _states.Add(EMovementState.Run, new RunState(_context, EMovementState.Run));
        _states.Add(EMovementState.Stop, new StopState(_context, EMovementState.Stop));
        
        _currentState = _states[EMovementState.Idle];
    }
    protected void Update()
    {
        base.Update();
        //Debug.Log("MovementStateMachine Update: " + _context.CharacterController.velocity.magnitude);
        if(_context.ShouldUpdateSpineTarget)
        {
            Debug.Log("MovementStateMachine Update: run or stop");
            _context.SpineTarget.localPosition  = new Vector3(_context.SpineTarget.localPosition.x, _context.SpineTarget.localPosition.y, _context.TargetTransform.localPosition.z);
        } else
        {
            _context.SpineTarget.localPosition = Vector3.Lerp(_context.SpineTarget.localPosition, _context.SpineTargetOriginalPosition, Time.deltaTime * 5f);
        }
        
    }
    
    private void FixedUpdate()
    {
        // Update character position using equation solver
        _context.UpdateTargetPosition();
    }
}

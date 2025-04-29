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
    [SerializeField] private float _spineReturnSpeed = 100f;
    [SerializeField] private float _spineStopRotationSpeed = 5f;
    [SerializeField] private float _maxSpineStopRotation = 40f;
    [SerializeField] private float _bendDuration = 0.5f;
    [SerializeField] private float _returnDuration = 0.3f;
    
    private MovementContext _context;
    private Vector3 _inputDirection;
    private bool _isRunning;
    private Quaternion _spineOriginalRotation;
    private bool _isInStopState = false;
    private bool _wasInStopState = false;
    private bool _canBend = true;
    private Coroutine _bendingCoroutine = null;
    
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
        
        // Store original spine rotation
       //_spineOriginalRotation = _spineTarget.localRotation;
        
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
        
        // Check if we're in the stop state
        // bool previousStopState = _isInStopState;
        // _isInStopState = _currentState is StopState;
        
        // // Detect first frame of entering stop state
        // if (_isInStopState && !_wasInStopState && _canBend)
        // {
        //     StartBendingAnimation();
        // }
        
        // // Update previous state tracking
        // _wasInStopState = _isInStopState;
        
        // // Reset _canBend when leaving stop state
        // if (!_isInStopState && previousStopState)
        // {
        //     _canBend = true;
        // }
        if(_context.ShouldUpdateSpineTarget)
        {
            _context.SpineTarget.localPosition = new Vector3(
                _context.SpineTarget.localPosition.x, 
                _context.SpineTarget.localPosition.y, 
                _context.TargetTransform.localPosition.z);
            
           
        }
    }
    
    private void StartBendingAnimation()
    {
        if (_bendingCoroutine != null)
        {
            StopCoroutine(_bendingCoroutine);
        }
        
        _canBend = false;
        _bendingCoroutine = StartCoroutine(BendingAnimationCoroutine());
    }
    
    private IEnumerator BendingAnimationCoroutine()
    {
        // Phase 1: Bend forward to max rotation
        float timer = 0f;
        while (timer < _bendDuration)
        {
            timer += Time.deltaTime;
            float bendProgress = Mathf.Clamp01(timer / _bendDuration);
            float easedProgress = EaseOutQuad(bendProgress);
            
            // Apply rotation
            Quaternion targetRotation = _spineOriginalRotation * Quaternion.Euler(_maxSpineStopRotation * easedProgress, 0, 0);
            _context.SpineTarget.localRotation = targetRotation;
            
            yield return null;
        }
        
        // Ensure we reach exactly the max rotation
        _context.SpineTarget.localRotation = _spineOriginalRotation * Quaternion.Euler(_maxSpineStopRotation, 0, 0);
        
        // Phase 2: Return to original rotation
        timer = 0f;
        while (timer < _returnDuration)
        {
            timer += Time.deltaTime;
            float returnProgress = Mathf.Clamp01(timer / _returnDuration);
            float easedProgress = EaseInOutQuad(returnProgress);
            
            // Apply rotation
            Quaternion currentMaxRotation = _spineOriginalRotation * Quaternion.Euler(_maxSpineStopRotation, 0, 0);
            _context.SpineTarget.localRotation = Quaternion.Slerp(currentMaxRotation, _spineOriginalRotation, easedProgress);
            
            yield return null;
        }
        
        // Ensure we return exactly to original rotation
        _context.SpineTarget.localRotation = _spineOriginalRotation;
        
        // Reset the coroutine reference
        _bendingCoroutine = null;
        
        // Note: We don't reset _canBend here - it will only reset when leaving the stop state
    }
    
    private void FixedUpdate()
    {
        // Update character position using equation solver
        _context.UpdateTargetPosition();
    }
    
    // Easing function for more natural movement - bend phase
    private float EaseOutQuad(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }
    
    // Easing function for smooth return
    private float EaseInOutQuad(float x)
    {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }
}

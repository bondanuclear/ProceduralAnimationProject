using System.Collections;
using System.Collections.Generic;
using Modules.Maths;
using UnityEngine;
//using System.Numerics;
public class SpiderController : MonoBehaviour
{
    [SerializeField] private float _distanceTillGroundHit = 0.1f;
    [SerializeField] private float MaxDistance = 0.3f;
    [Header("Ray settings: ")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask layerMask;
    [Header("Main body control: ")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float distanceFromGround;
    [SerializeField] private Transform centerOfRotation;
    [SerializeField] private float rotatingSpeed = 20f;
    [SerializeField] private bool autoRotateTowardsMovement = true;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Jump Settings:")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float fallSpeedMax = 20f;
    [SerializeField] private ParticleSystem jumpParticles;
    
    [Header("Parameters of the second order system: ")]
    [SerializeField] private float f;
    [SerializeField] private float z;
    [SerializeField] private float r;
    [Header("Equation Solver Configuration")]
    public Modules.Maths.EquationSolverType solverType = Modules.Maths.EquationSolverType.SemiImplicitEuler;
    public float frequency = 5f;
    public float damping = 0.5f;
    public float response = 0.3f;
    
    private IEquationSolver _equationSolver;
    private Vector3 targetMovePos;
    private Vector3 resultVector;
    
    // Rotation variables
    private Vector3 movementDirection;
    private Quaternion targetRotation;
    private float rotationVelocity;
    
    // Jump variables
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpCooldownTimer;
    [SerializeField] private float verticalVelocity;
    [SerializeField] private Vector3 originalTargetPos;
    
    void Awake()
    {
        // Initialize equation solver based on the configured type
        InitializeEquationSolver();
        
        // ... existing Awake code if any ...
    }
    
    private void InitializeEquationSolver()
    {
        // Create the equation solver based on the configured type
        switch (solverType)
        {
            case Modules.Maths.EquationSolverType.EulerStable:
                _equationSolver = new Modules.Maths.EulerStable(frequency, damping, response, transform.position);
                break;
            case Modules.Maths.EquationSolverType.EulerStableCorrectPhysics:
                _equationSolver = new Modules.Maths.EulerStableCorrectPhysics(frequency, damping, response, transform.position);
                break;
            case Modules.Maths.EquationSolverType.SemiImplicitEuler:
                _equationSolver = new Modules.Maths.SemiImplicitEuler(frequency, damping, response, transform.position);
                break;
            case Modules.Maths.EquationSolverType.VerletIntegration:
                _equationSolver = new Modules.Maths.VerletIntegration(frequency, damping, response, transform.position);
                break;
            default:
                _equationSolver = new Modules.Maths.SemiImplicitEuler(frequency, damping, response, transform.position);
                break;
        }
        
        Debug.Log($"Initialized {solverType} solver for {gameObject.name}");
    }
    
    private void Start() 
    {
        targetMovePos = transform.position;
        originalTargetPos = targetMovePos;
        targetRotation = transform.rotation;
        
        // Initialize jump variables
        isGrounded = true;
        jumpCooldownTimer = 0f;
    }
    
    private void Update()
    {
        // Movement variables
        Vector3 moveDirection = Vector3.zero;
        bool hasMovementInput = false;
        // Check if grounded
        CheckGrounded();
        ProcessMovement(ref moveDirection, ref hasMovementInput);
        ProcessJumping();
        ProcessRotation(moveDirection, hasMovementInput);
    }

    private void ProcessJumping()
    {
      
        // Update jump cooldown timer
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
          // Handle jumping
        HandleJumping();

        // Update ground height only when not jumping
        if (!isJumping)
        {
            if (Physics.Raycast(rayOrigin.position, Vector3.down, out RaycastHit info, rayLength, layerMask))
            {
                targetMovePos.y = info.point.y + distanceFromGround;
                originalTargetPos = targetMovePos;
            }
        }
        else
        {
            // Apply gravity to vertical velocity when jumping
            verticalVelocity -= gravityMultiplier * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, -fallSpeedMax);

            // Update vertical position with physics
            targetMovePos.y = originalTargetPos.y + verticalVelocity;

            // Check if we hit the ground while falling
            if (verticalVelocity < 0 && Physics.Raycast(rayOrigin.position, Vector3.down, out RaycastHit groundHit, _distanceTillGroundHit, layerMask))
            {
                isJumping = false;
                verticalVelocity = 0;
                targetMovePos.y = groundHit.point.y + distanceFromGround;
                originalTargetPos = targetMovePos;
            }
        }
    }

    private void ProcessMovement(ref Vector3 moveDirection, ref bool hasMovementInput)
    {
        // Process movement input - now relative to rotation
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.forward;
            hasMovementInput = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.back;
            hasMovementInput = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
            hasMovementInput = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
            hasMovementInput = true;
        }

        // Normalize for consistent speed in all directions
        if (moveDirection.magnitude > 0)
        {
            moveDirection.Normalize();

            // Convert local direction to world space based on current rotation
            Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);

            // Set the target position based on rotation
            targetMovePos += worldMoveDirection * Time.deltaTime * speed;

            // Store movement direction for auto-rotation
            movementDirection = worldMoveDirection;
        }
    }

    private void ProcessRotation(Vector3 moveDirection, bool hasMovementInput)
    {
        // Handle rotation input
        if (Input.GetKey(KeyCode.Q))
        {
            // Rotate left around up axis
            targetRotation *= Quaternion.Euler(0, -rotatingSpeed * Time.deltaTime, 0);
            autoRotateTowardsMovement = false; // Disable auto-rotation when manually rotating
        }
        else if (Input.GetKey(KeyCode.E))
        {
            // Rotate right around up axis
            targetRotation *= Quaternion.Euler(0, rotatingSpeed * Time.deltaTime, 0);
            autoRotateTowardsMovement = false; // Disable auto-rotation when manually rotating
        }

        // Auto rotate towards movement direction if enabled and we have movement
        // This is now redundant since we're moving in the direction we're facing
        // But we'll keep it for when movement might come from other sources
        if (autoRotateTowardsMovement && hasMovementInput && moveDirection.z > 0)
        {
            // For simplicity, we'll only enable auto-rotation when moving forward
            // This prevents the spider from flipping 180° when backing up

            if (movementDirection.magnitude > 0.01f)
            {
                movementDirection.y = 0; // Ignore vertical changes for rotation
                float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
                targetRotation = Quaternion.Euler(0, targetAngle, 0);
            }
        }

        // Apply rotation with smoothing
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(rayOrigin.position, Vector3.down, MaxDistance, layerMask);
    }
    
    private void HandleJumping()
    {
        // Jump when Space is pressed, we're grounded, and cooldown is complete
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCooldownTimer <= 0)
        {
            Debug.Log("Jumping");
            // Set jumping state
            isJumping = true;
            isGrounded = false;
            jumpCooldownTimer = jumpCooldown;
            
            // Apply jump force
            verticalVelocity = jumpForce;
            
            // Play particles if assigned
            if (jumpParticles != null)
            {
                ParticleSystem particles = Instantiate(jumpParticles, transform.position, Quaternion.identity);
                particles.Play();
            }
        }
    }
    
    private void FixedUpdate() 
    {
        // Оновлюємо позицію за методом Верле
        transform.position = _equationSolver.UpdateValues(targetMovePos, null, Time.fixedDeltaTime);
    }
    
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(rayOrigin.position, 0.1f);
        Gizmos.DrawRay(rayOrigin.position, rayLength * Vector3.down );
        
        // Draw movement direction if in play mode
        if (Application.isPlaying && movementDirection.magnitude > 0.01f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, movementDirection.normalized * 2f);
            
            // Also draw local forward direction for reference
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
        
        // Visualize grounded state
        if (Application.isPlaying)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(rayOrigin.position - Vector3.up * 0.2f, 0.1f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Modules.Maths;
using UnityEngine;
//using System.Numerics;
public class SpiderController : MonoBehaviour
{
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
    [SerializeField] private float landingDampeningTime = 0.3f; // Smooth landing time
    [SerializeField] private float landingDetectionDistance = 0.5f; // Distance to start preparing for landing
    
    [Header("Parameters of the second order system: ")]
    [SerializeField] private float f;
    [SerializeField] private float z;
    [SerializeField] private float r;
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
    [SerializeField] private bool isLanding;
    [SerializeField] private float jumpCooldownTimer;
    [SerializeField] private float verticalVelocity;
    [SerializeField] private Vector3 originalTargetPos;
    private float landingTimer;
    private float landingStartHeight;
    private float landingTargetHeight;
    
    private void Start() 
    {
        _equationSolver = new SemiImplicitEuler(f,z,r, transform.position);
        //_equationSolver = new EulerStableCorrectPhysics(f,z,r, transform.position);
        //_equationSolver = new SecondOrderDynamicsVerlet(f, z, r, transform.position, Time.fixedDeltaTime);
        targetMovePos = transform.position;
        originalTargetPos = targetMovePos;
        targetRotation = transform.rotation;
        
        // Initialize jump variables
        isGrounded = true;
        isJumping = false;
        isLanding = false;
        jumpCooldownTimer = 0f;
        landingTimer = 0f;
    }
    
    private void Update() 
    {
        // Update jump cooldown timer
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
        
        // Check if grounded
        CheckGrounded();
        
        // Handle jumping and landing
        HandleJumping();
        HandleLanding();
            
        // Movement variables
        Vector3 moveDirection = Vector3.zero;
        bool hasMovementInput = false;
        
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
        
        // Update ground height when not jumping or landing
        if (!isJumping && !isLanding)
        {
            UpdateGroundHeight();
        }
        else if (isJumping)
        {
            // Apply gravity to vertical velocity when jumping
            verticalVelocity -= gravityMultiplier * Time.deltaTime;
            verticalVelocity = Mathf.Max(verticalVelocity, -fallSpeedMax);
            
            // Update vertical position with physics
            targetMovePos.y = originalTargetPos.y + verticalVelocity;
            
            // Predict landing - start landing process slightly before hitting the ground
            if (verticalVelocity < 0)
            {
                RaycastHit groundHit;
                if (Physics.Raycast(rayOrigin.position, Vector3.down, out groundHit, landingDetectionDistance, layerMask))
                {
                    StartLanding(groundHit.point.y + distanceFromGround);
                }
            }
        }
        
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

    private void UpdateGroundHeight()
    {
        RaycastHit info;
        if (Physics.Raycast(rayOrigin.position, Vector3.down, out info, rayLength, layerMask))
        {
            float targetHeight = info.point.y + distanceFromGround;
            
            // Always ensure the body is at the correct height above ground
            targetMovePos.y = targetHeight;
            originalTargetPos = targetMovePos;
        }
    }
    
    private void CheckGrounded()
    {
        // Only update grounded state when not in landing animation
        if (!isLanding)
        {
            isGrounded = Physics.Raycast(rayOrigin.position, Vector3.down, MaxDistance, layerMask);
        }
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
            isLanding = false;
            jumpCooldownTimer = jumpCooldown;
            
            // Apply jump force
            verticalVelocity = jumpForce;
            
            // Play particles if assigned
            if (jumpParticles != null)
            {
                jumpParticles.Play();
            }
        }
    }
    
    private void StartLanding(float groundHeight)
    {
        isJumping = false;
        isLanding = true;
        landingTimer = 0f;
        landingStartHeight = targetMovePos.y;
        landingTargetHeight = groundHeight;
        Debug.Log("Starting landing sequence from " + landingStartHeight + " to " + landingTargetHeight);
    }
    
    private void HandleLanding()
    {
        if (isLanding)
        {
            landingTimer += Time.deltaTime;
            float t = Mathf.Clamp01(landingTimer / landingDampeningTime);
            
            // Use smooth step for more natural landing
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            // Interpolate height during landing
            targetMovePos.y = Mathf.Lerp(landingStartHeight, landingTargetHeight, smoothT);
            
            // Landing complete
            if (t >= 1)
            {
                isLanding = false;
                isGrounded = true;
                verticalVelocity = 0;
                originalTargetPos = targetMovePos;
                
                // Ensure we're exactly at the target height
                targetMovePos.y = landingTargetHeight;
                
                Debug.Log("Landing complete");
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
        Gizmos.DrawRay(rayOrigin.position, rayLength * Vector3.down);
        
        // Draw landing detection range
        if (Application.isPlaying && isJumping)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(rayOrigin.position, Vector3.down * landingDetectionDistance);
        }
        
        // Draw movement direction if in play mode
        if (Application.isPlaying && movementDirection.magnitude > 0.01f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, movementDirection.normalized * 2f);
            
            // Also draw local forward direction for reference
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
        
        // Visualize grounded/jumping/landing state
        if (Application.isPlaying)
        {
            if (isGrounded)
                Gizmos.color = Color.green;
            else if (isJumping)
                Gizmos.color = Color.red;
            else if (isLanding)
                Gizmos.color = Color.yellow;
                
            Gizmos.DrawWireSphere(rayOrigin.position - Vector3.up * 0.2f, 0.1f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Modules.Maths;
using UnityEngine;
//using System.Numerics;
public class SpiderController : MonoBehaviour
{
    [Header("Ray settings: ")]
    [SerializeField] Transform rayOrigin;
    [SerializeField] float rayLength;
    [SerializeField] LayerMask layerMask;
    [Header("Main body control: ")]
    [SerializeField] float speed = 5;
    [SerializeField] float distanceFromGround;
    [SerializeField] Transform centerOfRotation;
    [SerializeField] float rotatingSpeed = 20f;
    [SerializeField] bool autoRotateTowardsMovement = true;
    [SerializeField] float rotationSmoothTime = 0.1f;

    [Header("Parameters of the second order system: ")]
    [SerializeField] float f;
    [SerializeField] float z;
    [SerializeField] float r;
    private IEquationSolver _equationSolver;
    private Vector3 targetMovePos;
    private Vector3 resultVector;
    
    // Rotation variables
    private Vector3 movementDirection;
    private Quaternion targetRotation;
    private float rotationVelocity;
    
    private void Start() 
    {
        _equationSolver = new SemiImplicitEuler(f,z,r, transform.position);
        //_equationSolver = new EulerStableCorrectPhysics(f,z,r, transform.position);
        //_equationSolver = new SecondOrderDynamicsVerlet(f, z, r, transform.position, Time.fixedDeltaTime);
        targetMovePos = transform.position;
        targetRotation = transform.rotation;
    }
    
    private void Update() 
    {
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
        
        // Update ground height
        if (Physics.Raycast(rayOrigin.position, Vector3.down, out RaycastHit info, rayLength, layerMask))
        {
            targetMovePos.y = info.point.y + distanceFromGround;
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
    }
}

using UnityEngine;

namespace Modules.Creatures
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public class HumanController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float acceleration = 5f; // Smooth acceleration for animation
        public float rotationSpeed = 10f;

        private Rigidbody rb;
        private Animator animator;
        private Vector3 moveDirection;
        private float currentSpeed; // Used for animation blending

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            HandleInput();
            HandleAnimation();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        }

        private void Move()
        {
            if (moveDirection.magnitude > 0.1f)
            {
                // Apply movement using Rigidbody
                rb.linearVelocity = moveDirection * moveSpeed;

                // Rotate character smoothly
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
            else
            {
                // Stop movement completely when no input
                rb.linearVelocity = Vector3.zero;
            }
        }

        private void HandleAnimation()
        {
            // Smoothly transition speed to avoid sudden jumps
            float targetSpeed = moveDirection.magnitude;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

            // Set Animator parameter (Speed should be between 0 and 1)
            animator.SetFloat("Speed", currentSpeed);
        }
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
namespace Modules.Creatures
{
    public class LegFix : MonoBehaviour
    {
    
        [Header("Parameters for IK")]
        [SerializeField] float targetHeight;
        [SerializeField] Transform targetSphere;
        [SerializeField] Transform footEnd;
        [SerializeField] float stepDistance = 3f;
        [SerializeField] float minStepDistance = 0.1f; // Minimum threshold to prevent jittering
        
        
        [SerializeField] LayerMask layerMask;
        
        Vector3 newPosition;
        Vector3 currentPosition;
        float lerp = 1f; // Initialize to 1 to ensure we start in grounded state
    
        [SerializeField] float speedOfLerping = 3f;
        [SerializeField] float targetRayLength = 0.5f;
        [SerializeField] Vector3 offset;

        [Header("Step Animation")]
        [SerializeField] float stepHeight = 0.5f;
        [SerializeField] AnimationCurve stepHeightCurve;
        
        float legOffset = 0;
        Vector3 startPosition;
        bool targetValid = false;
        float lastValidTargetTime = 0f;
        
        private void Awake() {
            newPosition = transform.position;
            currentPosition = transform.position;
            
            // Initialize curve if not set in inspector
            if (stepHeightCurve.keys.Length == 0)
            {
                stepHeightCurve = new AnimationCurve(
                    new Keyframe(0, 0),
                    new Keyframe(0.5f, 0.4f),
                    new Keyframe(1, 0)
                );
                stepHeightCurve.SmoothTangents(0, 0.2f);
                stepHeightCurve.SmoothTangents(1, 0.2f);
                stepHeightCurve.SmoothTangents(2, 0.2f);
            }
        }
        private void Update()
        {
            // Update target sphere position based on ground raycast
            targetValid = false;
            if(Physics.Raycast(targetSphere.position + offset, Vector3.down, out RaycastHit targetInfo, targetRayLength, layerMask))
            {
                targetSphere.position = targetInfo.point;
                targetValid = true;
                lastValidTargetTime = Time.time;
            }
            else if (Time.time - lastValidTargetTime > 0.5f)
            {
                // If we've had no valid target for a while, force the leg to ground
                ForceToGround();
                return;
            }
            
            float distanceToTarget = Vector3.Distance(footEnd.position, targetSphere.position);
            
            // Only start a new step if we're not currently stepping and distance is significant
            if(distanceToTarget > stepDistance && lerp >= 1f && targetValid)
            {
                // Store start position for the step arc
                startPosition = transform.position;
                legOffset = targetHeight + targetSphere.position.y;
                lerp = 0f;
            }
            
            // Skip small movements that might cause jitter
            if(distanceToTarget < minStepDistance && lerp >= 1f)
            {
                return;
            }

            if(lerp < 1f)
            {
                // Update target position during the step in case target moves
                currentPosition = new Vector3(targetSphere.position.x, legOffset, targetSphere.position.z);
                
                // Increment lerp value based on time
                lerp += Time.deltaTime * speedOfLerping;
                
                // Ensure lerp never exceeds 1
                if (lerp > 1f) 
                {
                    lerp = 1f;
                    // Once step is complete, make sure to update the final position
                    transform.position = currentPosition;
                    return;
                }
                
                // Calculate arch during step based on animation curve
                float heightProgress = stepHeightCurve.Evaluate(lerp);
                float verticalOffset = heightProgress * stepHeight;
                
                // Linear interpolation for forward movement
                Vector3 horizontalPosition = Vector3.Lerp(startPosition, currentPosition, lerp);
                
                // Add height offset for arc movement
                Vector3 stepPosition = horizontalPosition + new Vector3(0, verticalOffset, 0);
                
                transform.position = stepPosition;
            } 
            else
            {
                // When not stepping, keep foot planted
                Debug.Log("not stepping for " + transform.name);
                transform.position = currentPosition;
            }
        }
        
        private void ForceToGround()
        {
            // Emergency recovery - force the foot to find ground
            lerp = 1f;
            
            // Try to find ground directly below the foot
            if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit groundHit, 5f, layerMask))
            {
                currentPosition = groundHit.point + new Vector3(0, targetHeight, 0);
                transform.position = currentPosition;
            }
        }
        
        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(footEnd.position,(targetSphere.position - footEnd.position) );
            Gizmos.DrawRay(targetSphere.position +  offset, Vector3.down * targetRayLength);
            
            // Draw step trajectory
            if (Application.isPlaying && lerp < 1f)
            {
                Gizmos.color = Color.yellow;
                
                // Draw step arc with 10 segments
                Vector3 prevPoint = startPosition;
                for (int i = 1; i <= 10; i++)
                {
                    float t = i / 10f;
                    float heightT = stepHeightCurve.Evaluate(t);
                    
                    Vector3 horizontalPos = Vector3.Lerp(startPosition, currentPosition, t);
                    Vector3 arcPoint = horizontalPos + new Vector3(0, heightT * stepHeight, 0);
                    
                    Gizmos.DrawLine(prevPoint, arcPoint);
                    prevPoint = arcPoint;
                }
            }
        }
    }
}
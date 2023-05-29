using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LegFix : MonoBehaviour
{
   
    [Header("Parameters for IK")]
    [SerializeField] float targetHeight;
    [SerializeField] Transform targetSphere;
    [SerializeField] Transform footEnd;
    [SerializeField] float stepDistance = 3f;
    
    
    [SerializeField] LayerMask layerMask;
    
    Vector3 newPosition;
    Vector3 currentPosition;
    float lerp = Mathf.Infinity;
   
    [SerializeField] float speedOfLerping = 3f;
    [SerializeField] float targetRayLength = 0.5f;
    [SerializeField] Vector3 offset;

    float legOffset  = 0;
    private void Awake() {
        newPosition = transform.position;
        currentPosition = newPosition;
    }
    private void Update()
    {
        
        //Ray targetRay = new Ray(targetSphere.position, Vector3.down);
        if(Physics.Raycast(targetSphere.position + offset, Vector3.down, out RaycastHit targetInfo, targetRayLength, layerMask))
        {
            targetSphere.position = targetInfo.point;
            //Debug.Log(targetInfo.point + "point ");
        }
        if(Vector3.Distance(footEnd.position, targetSphere.position) > stepDistance )
        {
           
          
            legOffset = targetHeight + targetSphere.position.y;
            lerp = 0;
            
        }

        if(lerp < 1)
        {
            //Debug.Log("LERPING");
            currentPosition = new Vector3(targetSphere.position.x, legOffset, targetSphere.position.z);
            lerp += Time.deltaTime * speedOfLerping;
            //Debug.Log("lerping, lerp is " + lerp);
            transform.position = Vector3.Lerp(transform.position, currentPosition, lerp);
        } else
        {
            //Debug.Log("SHOULD FIX IN PLACE" + currentPosition);
            transform.position = currentPosition;
        }
        // TO DO 
        
        // fix the rotation of IK with hint
        
        


        
        
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        //Gizmos.DrawRay(bodyPosition.position + (bodyPosition.right + coef), Vector3.down * rayLength);
        //Gizmos.DrawSphere(newPosition, 0.1f);
        Gizmos.DrawRay(footEnd.position,(targetSphere.position - footEnd.position) );
        Gizmos.DrawRay(targetSphere.position +  offset, Vector3.down * targetRayLength);
    }
}

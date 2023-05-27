using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LegFix : MonoBehaviour
{
    // array of 2 targets
    // array of 2 spheres 
    // ?
    [SerializeField] float targetHeight;
    [SerializeField] Transform bodyPosition;
    [SerializeField] Transform targetSphere;
    [SerializeField] Transform footEnd;
    //[SerializeField] Vector3 rightForwardOffset;
    [SerializeField] float rayLength = 10f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Vector3 coef;
    [SerializeField] float stepDistance = 3f;
    Vector3 newPosition;
    Vector3 currentPosition;
    float lerp = Mathf.Infinity;
    Ray ray;
    [SerializeField] float speedOfLerping = 3f;
    [SerializeField] float targetRayLength = 0.5f;
    [SerializeField] Vector3 offset;
    bool canStep = true;
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
           
            //Debug.LogWarning($"DISTANCE IS TOO GREAT. {footEnd.position} - foot end position and {targetSphere.position} is target sphere position");
            //Debug.Log(Vector3.Distance(footEnd.position, targetSphere.position));
            //transform.position = new Vector3(targetSphere.position.x, transform.position.y, targetSphere.position.z);
            //currentPosition = new Vector3(targetSphere.position.x, transform.position.y, targetSphere.position.z);
            //canStep = false;
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
        // check the mechanics of the back legs
        // fix the rotation of IK with hint
        // implement second order system to the main body
        


        // ray = new Ray(bodyPosition.position + (bodyPosition.right + coef), Vector3.down );
        // if(Physics.Raycast(ray, out RaycastHit info, rayLength, layerMask))
        // {
        //     //transform.position = info.point;
        //     //transform.position = new Vector3(info.point.x, 0.58f, info.point.z);
        //     //Debug.Log(info.point + " INFO POINT ");
        //     if(Vector3.Distance(newPosition, info.point) > stepDistance)
        //     {
        //         lerp = 0;
        //         newPosition = new Vector3(info.point.x, transform.position.y, info.point.z);
        //     }
        // }
        // if(lerp < 1)
        // {
        //     currentPosition = newPosition;
        //     transform.position = Vector3.Lerp(currentPosition, newPosition, lerp);
        //     lerp += Time.deltaTime * speedOfLerping;
        // }
        
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        //Gizmos.DrawRay(bodyPosition.position + (bodyPosition.right + coef), Vector3.down * rayLength);
        //Gizmos.DrawSphere(newPosition, 0.1f);
        Gizmos.DrawRay(footEnd.position,(targetSphere.position - footEnd.position) );
        Gizmos.DrawRay(targetSphere.position +  offset, Vector3.down * targetRayLength);
    }
}

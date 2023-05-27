using System.Collections;
using System.Collections.Generic;
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
    [Header("Parameters of the second order system: ")]
    [SerializeField] float f;
    [SerializeField] float z;
    [SerializeField] float r;
    //float timer = Mathf.Infinity;
    private SecondOrderDynamics instance;
    private Vector3 targetMovePos;
    private Vector3 resultVector;
    
    private void Start() {
        instance = new SecondOrderDynamics(f,z,r, transform.position);
        targetMovePos = transform.position;
    }
    private void Update() {
        if (Input.GetKey(KeyCode.W)) 
        {
            
            targetMovePos.z += Time.deltaTime * speed;
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            
            targetMovePos.z -= Time.deltaTime * speed;
        }
        else if (Input.GetKey(KeyCode.A)) 
        {
            
            targetMovePos.x -= Time.deltaTime * speed;
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            
            targetMovePos.x += Time.deltaTime * speed;
        }
        if (Physics.Raycast(rayOrigin.position, Vector3.down, out RaycastHit info, rayLength, layerMask))
        {
            targetMovePos.y = info.point.y + distanceFromGround;
            //transform.rotation = Quaternion.FromToRotation(Vector3.up, info.normal);
            //Quaternion rs = Quaternion.FromToRotation(Vector3.up, info.normal);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rs, Time.deltaTime * 2);
            //Debug.DrawRay(info.point, info.normal * 3);
        }
        
        //Debug.Log(targetMovePos);
    }
    private void FixedUpdate() {
        // timer += Time.deltaTime;
        // if(timer > 3f)
        // {
        //     timer = 0;
        //     Debug.Log(" Second order dynamics is called "
        //      + instance.UpdateValues(Time.fixedDeltaTime, transform.position));
        // }
        //Debug.Log(instance.UpdateValues(Time.fixedDeltaTime, transform.position));
        
        //target.transform.position = instance.UpdateValues(Time.fixedDeltaTime, transform.position);
        transform.position = instance.UpdateValues(Time.fixedDeltaTime, targetMovePos);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(rayOrigin.position, 0.1f);
        Gizmos.DrawRay(rayOrigin.position, rayLength * Vector3.down );
    }
}

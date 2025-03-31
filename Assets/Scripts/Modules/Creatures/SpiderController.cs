using System.Collections;
using System.Collections.Generic;
using Modules.Math;
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
    [SerializeField] float rotatingSpeed = 4f;

    [Header("Parameters of the second order system: ")]
    [SerializeField] float f;
    [SerializeField] float z;
    [SerializeField] float r;
    private IEquationSolver _equationSolver;
    private Vector3 targetMovePos;
    private Vector3 resultVector;
    
    private void Start() 
    {
        _equationSolver = new SecondOrderDynamicsEuler(f,z,r, transform.position);
        //_equationSolver = new SecondOrderDynamicsVerlet(f, z, r, transform.position, Time.fixedDeltaTime);
        targetMovePos = transform.position;
       
        
    }
    private void Update() 
    {
        
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
    }
}

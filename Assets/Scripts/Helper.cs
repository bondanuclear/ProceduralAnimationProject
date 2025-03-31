using System.Collections;
using System.Collections.Generic;
using Modules.Math;
using UnityEngine;

public class Helper : MonoBehaviour
{
    
    [SerializeField] Transform target;
    [SerializeField] float f;
    [SerializeField] float z;
    [SerializeField] float r;
    private SecondOrderDynamicsEuler instance;
    private void Start() {
        instance = new SecondOrderDynamicsEuler(f, z, r, transform.position);
    }
    private void FixedUpdate() {
        //target.transform.position = instance.UpdateValuesEuler(Time.fixedDeltaTime, transform.position);
    }
}

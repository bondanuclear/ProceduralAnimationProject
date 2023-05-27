using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    
    [SerializeField]Transform target;
    [SerializeField] float f;
    [SerializeField] float z;
    [SerializeField] float r;
    private SecondOrderDynamics instance;
    private void Start() {
        instance = new SecondOrderDynamics(f, z, r, transform.position);
    }
    private void FixedUpdate() {
        target.transform.position = instance.UpdateValues(Time.fixedDeltaTime, transform.position);
    }
}

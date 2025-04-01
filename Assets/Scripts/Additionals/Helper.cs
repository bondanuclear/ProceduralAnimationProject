using System.Collections;
using System.Collections.Generic;
using Modules.Maths;
using UnityEngine;

public class Helper : MonoBehaviour
{
    
    [SerializeField] Transform target;
    [SerializeField] float f;
    [SerializeField] float z;
    [SerializeField] float r;
    private IEquationSolver instance;
    private void Start() {
        instance = new EulerStable(f, z, r, transform.position);
    }
    private void FixedUpdate() {
        target.transform.position = instance.UpdateValues(transform.position, null, Time.fixedDeltaTime );
    }
}

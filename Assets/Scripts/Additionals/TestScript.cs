using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] Transform testTransform;
    private void Start() {
        Debug.Log($"Test transform is at coordinates {testTransform.position}");
    }
}

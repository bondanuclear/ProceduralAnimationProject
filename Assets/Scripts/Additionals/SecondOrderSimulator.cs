using UnityEngine;
using System.Collections.Generic;

public class SecondOrderSimulator : MonoBehaviour
{
    [Header("Second Order Parameters")]
    [Range(0.1f, 10f)] public float frequency = 2f;
    [Range(0f, 2f)] public float damping = 0.5f;
    [Range(0f, 5f)] public float response = 1f;

    [Header("Simulation Settings")]
    public int maxDataPoints = 200;
    public float timeStep = 0.02f;

    private List<Vector2> dataPoints = new List<Vector2>();
    private float timeElapsed = 0f;
    
    private Vector3 y, yd;  // Position & velocity
    private float k1, k2, k3; // System coefficients
    private Vector3 xp, target = Vector3.zero;

    private void Start()
    {
        ResetSystem();
    }

    private void Update()
    {
        SimulateStep();
    }

    private void ResetSystem()
    {
        k1 = damping / (frequency * Mathf.PI);
        k2 = 1 / ((2 * Mathf.PI * frequency) * (2 * Mathf.PI * frequency));
        k3 = (response * damping) / (2 * Mathf.PI * frequency);

        y = target;
        yd = Vector3.zero;
        xp = target;
        
        dataPoints.Clear();
        timeElapsed = 0f;
    }

    private void SimulateStep()
    {
        // Euler's Method
        Vector3 xd = (target - xp) / timeStep;
        xp = target;

        y = y + timeStep * yd;
        yd = yd + timeStep * (target + k3 * xd - y - k1 * yd) / k2;

        // Store Data for Plot
        dataPoints.Add(new Vector2(timeElapsed, y.x));

        if (dataPoints.Count > maxDataPoints)
            dataPoints.RemoveAt(0);

        timeElapsed += timeStep;
    }

    public List<Vector2> GetGraphData() => dataPoints;
}

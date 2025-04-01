using UnityEngine;
using System.Collections.Generic;

public class SecondOrderGraph : MonoBehaviour
{
    [Header("Second Order Parameters")]
    [Range(0.1f, 10f)] public float frequency = 2f;  // Natural frequency
    [Range(0f, 2f)] public float damping = 0.5f;    // Damping ratio
    [Range(-5f, 5f)] public float response = 1f;    // Response strength

    [Header("Graph Settings")]
    public int resolution = 200;   // Number of points to compute
    public float maxTime = 5f;     // How long to simulate (seconds)

    private List<Vector2> dataPoints = new List<Vector2>();

    private void Start()
    {
        ComputeGraph();
    }

    private void OnValidate()
    {
        ComputeGraph(); // Recalculate when parameters change
    }

    private void ComputeGraph()
    {
        dataPoints.Clear();
        float omega_n = 2 * Mathf.PI * frequency;
        
        for (int i = 0; i < resolution; i++)
        {
            float t = i * (maxTime / resolution);
            float responseValue = ComputeResponse(t, omega_n);
            dataPoints.Add(new Vector2(t, responseValue));
        }
    }

    private float ComputeResponse(float t, float omega_n)
    {
        float zeta = damping;
        float omega_d = omega_n * Mathf.Sqrt(1 - zeta * zeta);

        if (zeta < 1) // Underdamped case (oscillations)
        {
            float A = response; 
            return A * Mathf.Exp(-zeta * omega_n * t) * Mathf.Cos(omega_d * t);
        }
        else if (zeta == 1) // Critically damped
        {
            return response * (1 + omega_n * t) * Mathf.Exp(-omega_n * t);
        }
        else // Overdamped (no oscillation)
        {
            float lambda1 = -zeta * omega_n + omega_d;
            float lambda2 = -zeta * omega_n - omega_d;
            return response * (Mathf.Exp(lambda1 * t) + Mathf.Exp(lambda2 * t));
        }
    }

    public List<Vector2> GetGraphData() => dataPoints;
}

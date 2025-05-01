using UnityEngine;
using System.Collections.Generic;

public class SecondOrderTheoretical : MonoBehaviour
{
    [Header("Second Order Parameters")]
    [Range(0.1f, 10f)] public float frequency = 2f;
    [Range(0f, 2f)] public float damping = 0.5f;
    [Range(-5f, 5f)] public float response = 1f;

    [Header("Visualization Settings")]
    public int resolution = 200;
    public float simulationTime = 2f;
    
    private List<Vector2> dataPoints = new List<Vector2>();

    private void OnValidate()
    {
        // Recalculate response curve when parameters change
        CalculateResponseCurve();
    }

    private void Start()
    {
        CalculateResponseCurve();
    }

    public void CalculateResponseCurve()
    {
        dataPoints.Clear();
        
        // Calculate the natural frequency in radians
        float omega = 2f * Mathf.PI * frequency;
        
        // Calculate time step
        float dt = simulationTime / resolution;
        
        // Initial conditions (step response)
        float x = 0f;   // Position
        float v = 0f;   // Velocity
        
        // Add initial point
        dataPoints.Add(new Vector2(0f, x));
        
        // Calculate response over time
        for (int i = 1; i < resolution; i++)
        {
            float t = i * dt;
            
            // Different formulas depending on damping ratio
            if (damping < 1f) // Underdamped case
            {
                float dampedFreq = omega * Mathf.Sqrt(1f - damping * damping);
                float A = response;
                float expTerm = Mathf.Exp(-damping * omega * t);
                
                x = A * (1f - expTerm * (Mathf.Cos(dampedFreq * t) + 
                    (damping * omega / dampedFreq) * Mathf.Sin(dampedFreq * t)));
            }
            else if (damping == 1f) // Critically damped
            {
                x = response * (1f - Mathf.Exp(-omega * t) * (1f + omega * t));
            }
            else // Overdamped
            {
                float sqrt = Mathf.Sqrt(damping * damping - 1f);
                float expTerm = Mathf.Exp(-damping * omega * t);
                
                x = response * (1f - expTerm * (
                    (float)System.Math.Cosh(omega * sqrt * t) + 
                    (damping / sqrt) * (float)System.Math.Sinh(omega * sqrt * t)
                ));
            }
            
            dataPoints.Add(new Vector2(t, x));
        }
    }

    public List<Vector2> GetResponseCurve()
    {
        return dataPoints;
    }
} 
using UnityEngine;
using Modules.Maths;
using System.Collections.Generic;

public class SecondOrderComparison : MonoBehaviour
{
    [Header("Second Order Parameters")]
    [Range(0.1f, 10f)] public float frequency = 2f;
    [Range(0f, 2f)] public float damping = 0.5f;
    [Range(-5f, 5f)] public float response = 1f;

    [Header("Simulation Settings")]
    public int resolution = 200;
    public float simulationTime = 2f;
    public bool autoSimulate = true;
    
    [Header("Visualization")]
    public bool showTheoreticalCurve = true;
    public bool showEulerStable = true;
    public bool showEulerStableCorrectPhysics = true;
    public bool showSemiImplicitEuler = true;
    public bool showVerletIntegration = true;
    
    // Simulation data
    private Dictionary<EquationSolverType, List<Vector2>> solverData = new Dictionary<EquationSolverType, List<Vector2>>();
    private List<Vector2> theoreticalData = new List<Vector2>();
    
    private bool hasSimulated = false;
    
    private void OnValidate()
    {
        if (autoSimulate)
        {
            RunSimulation();
        }
    }
    
    private void Start()
    {
        RunSimulation();
        hasSimulated = true;
    }
    
    public void RunSimulation()
    {
        // Clear existing data
        solverData.Clear();
        theoreticalData.Clear();
        
        // Calculate time step
        float dt = simulationTime / resolution;
        
        // Initialize solver data structures
        foreach (EquationSolverType type in System.Enum.GetValues(typeof(EquationSolverType)))
        {
            solverData[type] = new List<Vector2>();
        }
        
        // Calculate theoretical curve
        CalculateTheoreticalResponse();
        
        // Simulate each solver type
        SimulateSolvers(dt);
        
        hasSimulated = true;
    }
    
    private void CalculateTheoreticalResponse()
    {
        // Calculate theoretical response
        float omega = 2f * Mathf.PI * frequency;
        float dt = simulationTime / resolution;
        
        // Initial point
        theoreticalData.Add(new Vector2(0f, 0f));
        
        // Calculate response over time using theoretical formulas
        for (int i = 1; i < resolution; i++)
        {
            float t = i * dt;
            float x = 0f;
            
            // Different formulas depending on damping ratio
            if (damping < 1f) // Underdamped case
            {
                float dampedFreq = omega * Mathf.Sqrt(1f - damping * damping);
                float expTerm = Mathf.Exp(-damping * omega * t);
                
                x = response * (1f - expTerm * (Mathf.Cos(dampedFreq * t) + 
                    (damping * omega / dampedFreq) * Mathf.Sin(dampedFreq * t)));
            }
            else if (Mathf.Approximately(damping, 1f)) // Critically damped
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
            
            theoreticalData.Add(new Vector2(t, x));
        }
    }
    
    private void SimulateSolvers(float dt)
    {
        // Create time array
        float[] timePoints = new float[resolution];
        for (int i = 0; i < resolution; i++)
        {
            timePoints[i] = i * dt;
        }
        
        // Simulate each solver
        foreach (EquationSolverType type in System.Enum.GetValues(typeof(EquationSolverType)))
        {
            // Create solver
            IEquationSolver solver = null;
            switch (type)
            {
                case EquationSolverType.EulerStable:
                    solver = new EulerStable(frequency, damping, response, Vector3.zero);
                    break;
                case EquationSolverType.EulerStableCorrectPhysics:
                    solver = new EulerStableCorrectPhysics(frequency, damping, response, Vector3.zero);
                    break;
                case EquationSolverType.SemiImplicitEuler:
                    solver = new SemiImplicitEuler(frequency, damping, response, Vector3.zero);
                    break;
                case EquationSolverType.VerletIntegration:
                    solver = new VerletIntegration(frequency, damping, response, Vector3.zero);
                    break;
            }
            
            if (solver == null) continue;
            
            List<Vector2> data = solverData[type];
            
            // Initial point
            data.Add(new Vector2(0f, 0f));
            
            // Target position (step input at x=1)
            Vector3 target = new Vector3(response, 0f, 0f);
            Vector3 currentPos = Vector3.zero;
            Vector3 lastPos = Vector3.zero;
            
            // Simulate
            for (int i = 1; i < resolution; i++)
            {
                // Calculate velocity
                Vector3 velocity = (currentPos - lastPos) / dt;
                lastPos = currentPos;
                
                // Update with current solver
                currentPos = solver.UpdateValues(target, velocity, dt);
                
                // Store result
                data.Add(new Vector2(timePoints[i], currentPos.x));
            }
        }
    }
    
    // Methods to access data
    public List<Vector2> GetTheoreticalData() => theoreticalData;
    
    public List<Vector2> GetSolverData(EquationSolverType type)
    {
        if (!hasSimulated)
        {
            RunSimulation();
        }
        
        if (solverData.ContainsKey(type))
        {
            return solverData[type];
        }
        
        return new List<Vector2>();
    }
    
    public Dictionary<EquationSolverType, List<Vector2>> GetAllSolverData() => solverData;
} 
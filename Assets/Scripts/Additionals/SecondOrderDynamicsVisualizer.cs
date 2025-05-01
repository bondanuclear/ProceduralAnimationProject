using UnityEngine;
using Modules.Maths;
using System.Collections.Generic;
using UnityEditor;

public class SecondOrderDynamicsVisualizer : MonoBehaviour
{
    [Header("Second Order Parameters")]
    [Range(0.1f, 10f)] public float f = 4.5f; // Frequency
    [Range(0f, 2f)] public float z = 0.35f;   // Damping
    [Range(-5f, 5f)] public float r = -3.5f;  // Response

    [Header("Visualization Settings")]
    public int resolution = 100;
    public float duration = 2f;
    
    [Header("Solver Selection")]
    public EquationSolverType solverType = EquationSolverType.SemiImplicitEuler;
    
    private List<float> simulationData = new List<float>();
    private bool needsRecalculation = true;
    
    // For drawing in inspector
    [HideInInspector] public Rect graphRect;
    
    private void OnValidate()
    {
        needsRecalculation = true;
    }
    
    private void Start()
    {
        RunSimulation();
    }
    
    public void RunSimulation()
    {
        simulationData.Clear();
        
        // Create solver
        IEquationSolver solver = null;
        switch (solverType)
        {
            case EquationSolverType.EulerStable:
                solver = new EulerStable(f, z, r, Vector3.zero);
                break;
            case EquationSolverType.EulerStableCorrectPhysics:
                solver = new EulerStableCorrectPhysics(f, z, r, Vector3.zero);
                break;
            case EquationSolverType.SemiImplicitEuler:
                solver = new SemiImplicitEuler(f, z, r, Vector3.zero);
                break;
            case EquationSolverType.VerletIntegration:
                solver = new VerletIntegration(f, z, r, Vector3.zero);
                break;
            default:
                solver = new SemiImplicitEuler(f, z, r, Vector3.zero);
                break;
        }

        // Initial position (x=0)
        Vector3 currentPos = Vector3.zero;
        Vector3 previousPos = Vector3.zero;
        
        // Target position (x=1)
        Vector3 targetPos = Vector3.right;
        
        // Time step
        float dt = duration / resolution;
        
        // Add initial point
        simulationData.Add(0);
        
        // Run simulation
        for (int i = 1; i <= resolution; i++)
        {
            // Calculate velocity (for solvers that use it)
            Vector3 velocity = (currentPos - previousPos) / dt;
            previousPos = currentPos;
            
            // Step the solver
            currentPos = solver.UpdateValues(targetPos, velocity, dt);
            
            // Store value (just x)
            simulationData.Add(currentPos.x);
        }
        
        needsRecalculation = false;
    }
    
    public List<float> GetSimulationData()
    {
        if (needsRecalculation)
            RunSimulation();
            
        return simulationData;
    }
} 
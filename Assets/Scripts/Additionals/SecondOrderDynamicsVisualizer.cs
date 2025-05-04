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
    public float duration = 6f;
    
    [Header("Solver Selection")]
    public EquationSolverType solverType = EquationSolverType.SemiImplicitEuler;
    
    [Header("Simulation Mode")]
    public bool useFixedDeltaTime = false;  // Toggle to use Time.fixedDeltaTime
    
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

        float initialX = r < 0 ? r / (1 - r) : 0;
        Vector3 currentPos = new Vector3(initialX, 0, 0);
        Vector3 previousPos = currentPos;
        
        // Target position (x=1)
        Vector3 targetPos = Vector3.right;
        
        // Get the time step based on the selected mode
        float dt = useFixedDeltaTime ? Time.fixedDeltaTime : duration / resolution;
        
        // Calculate number of steps based on the time step
        int steps = useFixedDeltaTime ? Mathf.FloorToInt(duration / dt) : resolution;
        
        // Add initial point
        simulationData.Add(0);
        
        // Run simulation
        for (int i = 1; i <= steps; i++)
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
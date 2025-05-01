using UnityEngine;
using Modules.Maths;
using System.Collections.Generic;

public class SecondOrderVisualizer : MonoBehaviour
{
    [Header("Second Order Parameters")]
    [Range(0.1f, 10f)] public float frequency = 2f;
    [Range(0f, 2f)] public float damping = 0.5f;
    [Range(-5f, 5f)] public float response = 1f;

    [Header("Visualization Settings")]
    public Transform target;
    public int resolution = 256;
    public float simulationTime = 2f;
    public EquationSolverType solverType = EquationSolverType.SemiImplicitEuler;

    // Internal variables
    private Dictionary<EquationSolverType, IEquationSolver> solvers = new Dictionary<EquationSolverType, IEquationSolver>();
    private Dictionary<EquationSolverType, List<float>> dataPoints = new Dictionary<EquationSolverType, List<float>>();
    
    private Vector3 initialPosition;
    private Vector3 previousTargetPosition;
    private bool hasInitialized = false;

    private void Start()
    {
        if (target == null)
        {
            target = transform;
            Debug.LogWarning("No target assigned for SecondOrderVisualizer. Using self as target.");
        }
        
        initialPosition = transform.position;
        previousTargetPosition = target.position;
        
        InitializeSolvers();
        hasInitialized = true;
    }

    private void Update()
    {
        // Detect changes in parameters to reinitialize solvers
        if (GUI.changed)
        {
            InitializeSolvers();
        }
        
        // Update using active solver only
        SimulateWithCurrentSolver(Time.deltaTime);
    }

    public void InitializeSolvers()
    {
        // Clear existing solvers and data
        solvers.Clear();
        dataPoints.Clear();

        // Create all solver types
        foreach (EquationSolverType type in System.Enum.GetValues(typeof(EquationSolverType)))
        {
            // Create the solver with current parameters
            IEquationSolver solver = null;
            switch (type)
            {
                case EquationSolverType.EulerStable:
                    solver = new EulerStable(frequency, damping, response, initialPosition);
                    break;
                case EquationSolverType.EulerStableCorrectPhysics:
                    solver = new EulerStableCorrectPhysics(frequency, damping, response, initialPosition);
                    break;
                case EquationSolverType.SemiImplicitEuler:
                    solver = new SemiImplicitEuler(frequency, damping, response, initialPosition);
                    break;
                case EquationSolverType.VerletIntegration:
                    solver = new VerletIntegration(frequency, damping, response, initialPosition);
                    break;
            }

            if (solver != null)
            {
                solvers[type] = solver;
                dataPoints[type] = new List<float>();
            }
        }
    }

    private void SimulateWithCurrentSolver(float deltaTime)
    {
        if (!hasInitialized || target == null) return;

        // Get the current solver
        if (!solvers.ContainsKey(solverType))
        {
            Debug.LogError($"Solver of type {solverType} not initialized!");
            return;
        }

        // Calculate velocity if needed
        Vector3 targetVelocity = (target.position - previousTargetPosition) / deltaTime;
        previousTargetPosition = target.position;

        // Update position with current solver
        Vector3 updatedPosition = solvers[solverType].UpdateValues(target.position, targetVelocity, deltaTime);
        transform.position = updatedPosition;

        // Store data point (using X coordinate for visualization)
        List<float> currentDataPoints = dataPoints[solverType];
        if (currentDataPoints.Count >= resolution)
        {
            currentDataPoints.RemoveAt(0);
        }
        currentDataPoints.Add(updatedPosition.x);
    }

    // Method to get all data points for a specific solver
    public List<float> GetDataPoints(EquationSolverType type)
    {
        if (dataPoints.ContainsKey(type))
        {
            return dataPoints[type];
        }
        return new List<float>();
    }

    // Method to get data points for current solver
    public List<float> GetCurrentDataPoints()
    {
        return GetDataPoints(solverType);
    }

    // Reset the position and clear data
    public void ResetSimulation()
    {
        transform.position = initialPosition;
        InitializeSolvers();
    }
} 
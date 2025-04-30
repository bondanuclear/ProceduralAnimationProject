using UnityEngine;

namespace Modules.Maths
{
    /// <summary>
    /// Parameters for equation solvers
    /// </summary>
    public struct EquationSolverParameters
    {
        public float Frequency;
        public float Damping;
        public float Response;
        public Vector3 InitialPosition;
        
        public EquationSolverParameters(float frequency, float damping, float response, Vector3 initialPosition)
        {
            Frequency = frequency;
            Damping = damping;
            Response = response;
            InitialPosition = initialPosition;
        }
        
        // Default parameters
        public static EquationSolverParameters Default => new EquationSolverParameters(5f, 0.5f, 0.3f, Vector3.zero);
    }

    /// <summary>
    /// Factory class to create equation solvers
    /// </summary>
    public static class EquationSolverFactory
    {
        public static IEquationSolver CreateSolver(EquationSolverType solverType, EquationSolverParameters parameters)
        {
            return solverType switch
            {
                EquationSolverType.EulerStable => new EulerStable(parameters.Frequency, parameters.Damping, parameters.Response, parameters.InitialPosition),
                EquationSolverType.EulerStableCorrectPhysics => new EulerStableCorrectPhysics(parameters.Frequency, parameters.Damping, parameters.Response, parameters.InitialPosition),
                EquationSolverType.SemiImplicitEuler => new SemiImplicitEuler(parameters.Frequency, parameters.Damping, parameters.Response, parameters.InitialPosition),
                EquationSolverType.VerletIntegration => new VerletIntegration(parameters.Frequency, parameters.Damping, parameters.Response, parameters.InitialPosition),
                _ => new SemiImplicitEuler(parameters.Frequency, parameters.Damping, parameters.Response, parameters.InitialPosition) // Default
            };
        }
        
        // Overload with default parameters
        public static IEquationSolver CreateSolver(EquationSolverType solverType)
        {
            return CreateSolver(solverType, EquationSolverParameters.Default);
        }
    }

    /// <summary>
    /// Enum for the different equation solver types
    /// </summary>
    public enum EquationSolverType
    {
        EulerStable,
        EulerStableCorrectPhysics,
        SemiImplicitEuler,
        VerletIntegration
    }
} 
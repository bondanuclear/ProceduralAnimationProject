using UnityEngine;

namespace Modules.Maths
{
    /// <summary>
    /// Base equation solver implementation
    /// </summary>
    public abstract class EquationSolverBase : IEquationSolver
    {
        public abstract Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity);
    }

    /// <summary>
    /// Euler Stable solver implementation
    /// </summary>
    public class EulerStableSolver : EquationSolverBase
    {
        public override Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
        {
            // Euler Stable implementation
            Vector3 velocity = xd ?? Vector3.zero;
            Vector3 acceleration = (T == Mathf.Infinity) ? Vector3.zero : (x - velocity * T) / (T * T);
            return x + velocity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Euler Stable with Correct Physics solver implementation
    /// </summary>
    public class EulerStableCorrectPhysicsSolver : EquationSolverBase
    {
        public override Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
        {
            // Euler Stable with Correct Physics implementation
            Vector3 velocity = xd ?? Vector3.zero;
            Vector3 acceleration = (T == Mathf.Infinity) ? Vector3.zero : (x - velocity * T) / (T * T);
            return x + velocity * Time.deltaTime + 0.5f * acceleration * Time.deltaTime * Time.deltaTime;
        }
    }

    /// <summary>
    /// Semi-Implicit Euler solver implementation
    /// </summary>
    public class SemiImplicitEulerSolver : EquationSolverBase
    {
        public override Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
        {
            // Semi-Implicit Euler implementation
            Vector3 velocity = xd ?? Vector3.zero;
            Vector3 acceleration = (T == Mathf.Infinity) ? Vector3.zero : (x - velocity * T) / (T * T);
            Vector3 newVelocity = velocity + acceleration * Time.deltaTime;
            return x + newVelocity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Verlet Integration solver implementation
    /// </summary>
    public class VerletIntegrationSolver : EquationSolverBase
    {
        private Vector3 previousPosition;
        private bool initialized = false;

        public override Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
        {
            // Verlet Integration implementation
            Vector3 velocity = xd ?? Vector3.zero;
            Vector3 acceleration = (T == Mathf.Infinity) ? Vector3.zero : (x - velocity * T) / (T * T);
            
            if (!initialized)
            {
                previousPosition = x - velocity * Time.deltaTime;
                initialized = true;
            }
            
            Vector3 newPosition = 2 * x - previousPosition + acceleration * Time.deltaTime * Time.deltaTime;
            previousPosition = x;
            
            return newPosition;
        }
    }

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
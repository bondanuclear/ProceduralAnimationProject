using UnityEngine;
using Modules.Maths;
using System.Reflection;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Component that allows changing the equation solver for a character in the editor
/// </summary>
public class EquationSolverController : MonoBehaviour
{
    [Header("Equation Solver Settings")]
    [SerializeField] public EquationSolverType solverType = EquationSolverType.SemiImplicitEuler;
    [SerializeField] public float frequency = 5.0f;
    [SerializeField] public float damping = 0.5f;
    [SerializeField] public float response = 0.3f;
    
    // Private properties for reflection access
    private SpiderController _spiderController;
    private object _movementStateMachine;
    private object _movementContext;
    
    private void OnValidate()
    {
        // When values change in the inspector, apply the new solver
        ApplySolverToComponents();
    }
    
    // private void Start()
    // {
    //     // Apply solver on start
    //     ApplySolverToComponents();
    // }
    
    /// <summary>
    /// Apply the configured equation solver to all relevant components
    /// </summary>
    public void ApplySolverToComponents()
    {
        var parameters = new EquationSolverParameters(frequency, damping, response, transform.position);
        var newSolver = EquationSolverFactory.CreateSolver(solverType, parameters);
        
        // Try to find SpiderController
        if (_spiderController == null)
        {
            _spiderController = GetComponent<SpiderController>();
        }
        
        if (_spiderController != null)
        {
            // Try to set values directly first
            if (_spiderController.GetType().GetProperty("solverType") != null)
            {
                _spiderController.solverType = solverType;
                _spiderController.frequency = frequency;
                _spiderController.damping = damping;
                _spiderController.response = response;
            }
            else
            {
                // Fall back to reflection for older versions
                SetFieldValue(_spiderController, "_equationSolver", newSolver);
            }
            Debug.Log($"Applied {solverType} solver to SpiderController");
        }
        
        // Look for MovementStateMachine component
        var movementStateMachine = GetComponent<MovementStateMachine>();
        if (movementStateMachine != null)
        {
            // Try to set values directly first
            if (movementStateMachine.GetType().GetProperty("solverType") != null)
            {
                movementStateMachine.solverType = solverType;
                movementStateMachine.frequency = frequency;
                movementStateMachine.damping = damping;
                movementStateMachine.response = response;
            }
            else
            {
                // Fall back to reflection for older versions
                SetFieldValue(movementStateMachine, "_equationSolver", newSolver);
            }
            Debug.Log($"Applied {solverType} solver to MovementStateMachine");
        }
        
        // Look for components with "Rig" in their name
        Component[] components = GetComponentsInChildren<Component>();
        foreach (var component in components)
        {
            if (component.GetType().Name.Contains("Rig"))
            {
                FieldInfo equationSolverField = FindEquationSolverField(component.GetType());
                if (equationSolverField != null)
                {
                    equationSolverField.SetValue(component, newSolver);
                    Debug.Log($"Applied {solverType} solver to {component.GetType().Name}");
                }
            }
        }
        
        // Try to find MovementStateMachine or similar components
        foreach (var component in components)
        {
            if (component.GetType().Name.Contains("Movement") || 
                component.GetType().Name.Contains("State"))
            {
                FieldInfo equationSolverField = FindEquationSolverField(component.GetType());
                if (equationSolverField != null)
                {
                    equationSolverField.SetValue(component, newSolver);
                    _movementStateMachine = component;
                    Debug.Log($"Applied {solverType} solver to {component.GetType().Name}");
                }
                
                // Look for MovementContext field
                FieldInfo contextField = FindMovementContextField(component.GetType());
                if (contextField != null)
                {
                    object context = contextField.GetValue(component);
                    if (context != null)
                    {
                        _movementContext = context;
                        FieldInfo contextSolverField = FindEquationSolverField(context.GetType());
                        if (contextSolverField != null)
                        {
                            contextSolverField.SetValue(context, newSolver);
                            Debug.Log($"Applied {solverType} solver to MovementContext");
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Find a field that holds an IEquationSolver
    /// </summary>
    private FieldInfo FindEquationSolverField(System.Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
        // First look for name matching _equationSolver
        foreach (var field in fields)
        {
            if (field.Name == "_equationSolver")
            {
                return field;
            }
        }
        
        // Then look for fields containing "equationSolver" in the name
        foreach (var field in fields)
        {
            if (field.Name.ToLower().Contains("equationsolver"))
            {
                return field;
            }
        }
        
        // Finally look for fields of type IEquationSolver
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(IEquationSolver) || 
                field.FieldType.GetInterface("IEquationSolver") != null)
            {
                return field;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Find a field that might hold a MovementContext
    /// </summary>
    private FieldInfo FindMovementContextField(System.Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        
        foreach (var field in fields)
        {
            if (field.Name.Contains("context") || field.Name.Contains("Context"))
            {
                return field;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Set a field value using reflection
    /// </summary>
    private void SetFieldValue(object target, string fieldName, object value)
    {
        if (target == null) return;
        
        FieldInfo field = target.GetType().GetField(fieldName, 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }
} 
using UnityEngine;
using UnityEditor;
using Modules.Maths;

[CustomEditor(typeof(EquationSolverController))]
public class EquationSolverControllerEditor : Editor
{
    private SerializedProperty solverTypeProp;
    private SerializedProperty frequencyProp;
    private SerializedProperty dampingProp;
    private SerializedProperty responseProp;
    
    private EquationSolverController controller;
    private GUIStyle headerStyle;
    private GUIStyle descriptionStyle;
    
    private void OnEnable()
    {
        // Get serialized properties
        solverTypeProp = serializedObject.FindProperty("solverType");
        frequencyProp = serializedObject.FindProperty("frequency");
        dampingProp = serializedObject.FindProperty("damping");
        responseProp = serializedObject.FindProperty("response");
        
        // Get the controller
        controller = (EquationSolverController)target;
        
        // Initialize custom styles
        headerStyle = new GUIStyle();
        descriptionStyle = new GUIStyle();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // Set up styles for GUI elements
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontSize = 14;
        headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        headerStyle.margin = new RectOffset(0, 0, 8, 8);
        
        descriptionStyle.wordWrap = true;
        descriptionStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : Color.black;
        descriptionStyle.margin = new RectOffset(0, 0, 4, 8);
        
        // Display header
        EditorGUILayout.LabelField("Equation Solver Controller", headerStyle);
        EditorGUILayout.LabelField("This component allows you to change the equation solver method used by this character's procedural animation components.", descriptionStyle);
        
        EditorGUILayout.Space(10);
        
        // Display solver type with descriptions
        EditorGUILayout.PropertyField(solverTypeProp, new GUIContent("Solver Type", "The mathematical method used to solve second-order dynamics equations."));
        DisplaySolverDescription((EquationSolverType)solverTypeProp.enumValueIndex);
        
        EditorGUILayout.Space(10);
        
        // Display parameters with sliders and tooltips
        EditorGUILayout.LabelField("Solver Parameters", headerStyle);
        
        EditorGUILayout.PropertyField(frequencyProp, new GUIContent("Frequency", "Controls how quickly the system oscillates (higher values = faster oscillation)."));
        frequencyProp.floatValue = EditorGUILayout.Slider(frequencyProp.floatValue, 1f, 20f);
        
        EditorGUILayout.PropertyField(dampingProp, new GUIContent("Damping", "Controls how quickly oscillations die out (higher values = faster stabilization)."));
        dampingProp.floatValue = EditorGUILayout.Slider(dampingProp.floatValue, 0.1f, 2f);
        
        EditorGUILayout.PropertyField(responseProp, new GUIContent("Response", "Controls how responsive the system is to input changes (higher values = more responsive)."));
        responseProp.floatValue = EditorGUILayout.Slider(responseProp.floatValue, 0.1f, 1f);
        
        EditorGUILayout.Space(15);
        
        // Add apply button
        if (GUILayout.Button("Apply Settings", GUILayout.Height(30)))
        {
            serializedObject.ApplyModifiedProperties();
            controller.ApplySolverToComponents();
        }
        
        EditorGUILayout.Space(5);
        
        // Add preset buttons
        EditorGUILayout.LabelField("Presets", headerStyle);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Smooth Motion"))
        {
            frequencyProp.floatValue = 3.5f;
            dampingProp.floatValue = 0.8f;
            responseProp.floatValue = 0.4f;
        }
        
        if (GUILayout.Button("Responsive"))
        {
            frequencyProp.floatValue = 8f;
            dampingProp.floatValue = 0.4f;
            responseProp.floatValue = 0.7f;
        }
        
        if (GUILayout.Button("Bouncy"))
        {
            frequencyProp.floatValue = 6f;
            dampingProp.floatValue = 0.2f;
            responseProp.floatValue = 0.5f;
        }
        
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DisplaySolverDescription(EquationSolverType solverType)
    {
        string description = "";
        
        switch (solverType)
        {
            case EquationSolverType.EulerStable:
                description = "Stable Euler method. Simple, resource-efficient solver suitable for most cases.";
                break;
            case EquationSolverType.EulerStableCorrectPhysics:
                description = "Improved Euler method with better physics representation. More accurate but slightly more expensive.";
                break;
            case EquationSolverType.SemiImplicitEuler:
                description = "Semi-implicit Euler method. Good balance between accuracy and performance. Recommended default.";
                break;
            case EquationSolverType.VerletIntegration:
                description = "Verlet integration. Excellent stability and energy conservation, good for springy or oscillating movements.";
                break;
        }
        
        EditorGUILayout.HelpBox(description, MessageType.Info);
    }
} 
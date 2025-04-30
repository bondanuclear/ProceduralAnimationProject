using UnityEngine;
using UnityEditor;
using System.Linq;
using Modules.Maths;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;
using System;

public class CharacterEquationSolverEditor : EditorWindow
{
    private GameObject selectedCharacter;
    private EquationSolverType selectedSolverType = EquationSolverType.SemiImplicitEuler;
    private float frequency = 5f;
    private float damping = 0.5f;
    private float response = 0.3f;
    
    private Vector2 scrollPos;
    private List<GameObject> characterObjects = new List<GameObject>();
    
    [MenuItem("Tools/Character Equation Solver Editor")]
    public static void ShowWindow()
    {
        GetWindow<CharacterEquationSolverEditor>("Equation Solver Editor");
    }
    
    private void OnFocus()
    {
        FindCharactersInScene();
    }
    
    private void OnEnable()
    {
        FindCharactersInScene();
    }
    
    private void FindCharactersInScene()
    {
        characterObjects.Clear();
        
        // Find characters with CharacterController component
        CharacterController[] controllers = FindObjectsOfType<CharacterController>();
        foreach (var controller in controllers)
        {
            if (controller.gameObject != null)
            {
                characterObjects.Add(controller.gameObject);
            }
        }
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Character Equation Solver Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (characterObjects.Count == 0)
        {
            EditorGUILayout.HelpBox("No characters with CharacterController found in the scene.", MessageType.Info);
            if (GUILayout.Button("Refresh"))
            {
                FindCharactersInScene();
            }
            return;
        }
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        EditorGUILayout.LabelField("Select Character", EditorStyles.boldLabel);
        
        // Create string array of character names for the popup
        string[] characterNames = characterObjects.Select(c => c.name).ToArray();
        int selectedIndex = selectedCharacter != null ? Array.IndexOf(characterObjects.ToArray(), selectedCharacter) : -1;
        
        // Character selection popup
        int newSelectedIndex = EditorGUILayout.Popup("Character", selectedIndex >= 0 ? selectedIndex : 0, characterNames);
        if (newSelectedIndex != selectedIndex || selectedCharacter == null)
        {
            selectedCharacter = characterObjects[newSelectedIndex];
        }
        
        EditorGUILayout.Space();
        
        if (selectedCharacter != null)
        {
            EditorGUILayout.LabelField("Equation Solver Settings", EditorStyles.boldLabel);
            
            // Choose equation solver type
            selectedSolverType = (EquationSolverType)EditorGUILayout.EnumPopup("Solver Type", selectedSolverType);
            
            EditorGUILayout.Space();
            
            // Equation solver parameters
            frequency = EditorGUILayout.FloatField("Frequency", frequency);
            damping = EditorGUILayout.FloatField("Damping", damping);
            response = EditorGUILayout.FloatField("Response", response);
            
            EditorGUILayout.Space();
            
            // Apply button
            if (GUILayout.Button("Apply Equation Solver"))
            {
                ApplyEquationSolver();
            }
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void ApplyEquationSolver()
    {
        // Find components related to movement and check for Rigs
        Component[] components = selectedCharacter.GetComponentsInChildren<Component>();
        
        // Try to find components that likely use equation solvers
        bool solverApplied = false;
        
        // First check for SpiderController which directly uses the solver
        var spiderController = selectedCharacter.GetComponent<SpiderController>();
        if (spiderController != null)
        {
            // Set the parameters directly
            spiderController.solverType = selectedSolverType;
            spiderController.frequency = frequency;
            spiderController.damping = damping;
            spiderController.response = response;
            
            // Force reinitialize the equation solver
            spiderController.InitializeEquationSolver();
            
            solverApplied = true;
            Debug.Log($"Applied {selectedSolverType} solver to SpiderController on {selectedCharacter.name}");
        }
        
        // Check for MovementStateMachine
        var movementStateMachine = selectedCharacter.GetComponent<MovementStateMachine>();
        if (movementStateMachine != null)
        {
            // Set the parameters directly
            movementStateMachine.solverType = selectedSolverType;
            movementStateMachine.frequency = frequency;
            movementStateMachine.damping = damping;
            movementStateMachine.response = response;
            
            // Force reinitialize the equation solver
            movementStateMachine.InitializeEquationSolver();
            
            solverApplied = true;
            Debug.Log($"Applied {selectedSolverType} solver to MovementStateMachine on {selectedCharacter.name}");
        }
        
        // Also update the EquationSolverController if present
        var equationSolverController = selectedCharacter.GetComponent<EquationSolverController>();
        if (equationSolverController != null)
        {
            // Set the parameters directly
            equationSolverController.solverType = selectedSolverType;
            equationSolverController.frequency = frequency;
            equationSolverController.damping = damping;
            equationSolverController.response = response;
            
            // Apply the changes
            equationSolverController.ApplySolverToComponents();
            solverApplied = true;
            Debug.Log($"Applied {selectedSolverType} solver via EquationSolverController on {selectedCharacter.name}");
        }
        
        // Fallback to using reflection for components we don't have direct access to
        if (!solverApplied)
        {
            // Create parameters for the equation solver
            var parameters = new EquationSolverParameters(frequency, damping, response, selectedCharacter.transform.position);
            
            // Find components with "Rig" in their name
            var rigComponents = components.Where(c => c.GetType().Name.Contains("Rig")).ToList();
            foreach (var rig in rigComponents)
            {
                // Use reflection to find _equationSolver fields
                var fields = rig.GetType().GetFields(System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                
                foreach (var field in fields)
                {
                    if (field.Name.Contains("equationSolver") || field.FieldType == typeof(IEquationSolver))
                    {
                        var solver = EquationSolverFactory.CreateSolver(selectedSolverType, parameters);
                        field.SetValue(rig, solver);
                        solverApplied = true;
                        Debug.Log($"Applied {selectedSolverType} solver to {rig.GetType().Name} on {selectedCharacter.name}");
                    }
                }
            }
        }
        
        if (!solverApplied)
        {
            EditorUtility.DisplayDialog("Equation Solver Not Applied", 
                "Could not find any components with equation solvers to modify. Make sure the character has the proper components.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Equation Solver Applied", 
                $"Applied {selectedSolverType} solver to character {selectedCharacter.name}.", "OK");
        }
    }
} 
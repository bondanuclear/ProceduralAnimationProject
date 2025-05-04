using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Modules.Maths;
using UnityEditor.SceneManagement;
using System;

public class EditorCharacterSpawner : EditorWindow
{
    // Enum for equation solver types that matches what we need
    public enum EquationSolverType
    {
        EulerStable,
        EulerStableCorrectPhysics,
        SemiImplicitEuler,
        VerletIntegration
    }
    
    private GameObject humanPrefab;
    private GameObject spiderPrefab;
    private CharacterType selectedCharacterType = CharacterType.Human;
    private Transform spawnPoint;
    private Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
    private bool useCustomPosition = true;
    
    // For prefab selection
    private List<GameObject> availableHumanPrefabs = new List<GameObject>();
    private List<GameObject> availableSpiderPrefabs = new List<GameObject>();
    private int selectedHumanIndex = 0;
    private int selectedSpiderIndex = 0;
    
    // For procedural animation rigs
    private bool showProceduralSettings = true;
    private bool enableMovementRig = true;
    private bool enableHeadTrackingRig = true;
    private bool enableEnvironmentInteractionRig = true;
    private Dictionary<string, bool> customRigs = new Dictionary<string, bool>();
    private List<string> availableRigs = new List<string>();
    private Vector2 rigScrollPosition;
    
    // For equation solver selection
    private bool showEquationSolverSettings = true;
    private Modules.Maths.EquationSolverType selectedEquationSolverType = Modules.Maths.EquationSolverType.SemiImplicitEuler;
    private Dictionary<string, Modules.Maths.EquationSolverType> rigSolverTypes = new Dictionary<string, Modules.Maths.EquationSolverType>();
    
    // Equation solver parameters
    private bool showSolverParameters = true;
    private float frequency = 5f;
    private float damping = 0.5f;
    private float response = 0.3f;
    private Vector3 initialPosition = Vector3.zero;
    private Dictionary<string, Modules.Maths.EquationSolverParameters> rigSolverParameters = new Dictionary<string, Modules.Maths.EquationSolverParameters>();

    [MenuItem("Tools/Character Spawner")]
    public static void ShowWindow()
    {
        EditorCharacterSpawner window = GetWindow<EditorCharacterSpawner>("Character Spawner");
        window.minSize = new Vector2(300, 300);
    }

    private void OnEnable()
    {
        // Try to find prefabs automatically
        FindPrefabs();
        
        // Scan for available procedural rigs in the human prefab
        ScanForProceduralRigs();
    }
    
    private void FindPrefabs()
    {
        // Try direct paths first
        humanPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BananaManProcedural.prefab");
        spiderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SpiderProceduralAnimation.prefab");
        
        // If that fails, search more thoroughly
        if (humanPrefab == null || spiderPrefab == null)
        {
            var foundPrefabs = AutoPrefabAssigner.FindPrefabsByNames("Player", "Human", "Spider", "TestSpider");
            
            if (humanPrefab == null && foundPrefabs.ContainsKey("Player") && foundPrefabs["Player"] != null)
            {
                humanPrefab = foundPrefabs["Player"];
            }
            else if (humanPrefab == null && foundPrefabs.ContainsKey("Human") && foundPrefabs["Human"] != null)
            {
                humanPrefab = foundPrefabs["Human"];
            }
            
            if (spiderPrefab == null && foundPrefabs.ContainsKey("Spider") && foundPrefabs["Spider"] != null)
            {
                spiderPrefab = foundPrefabs["Spider"];
            }
            else if (spiderPrefab == null && foundPrefabs.ContainsKey("TestSpider") && foundPrefabs["TestSpider"] != null)
            {
                spiderPrefab = foundPrefabs["TestSpider"];
            }
        }
        
        // Find all available character prefabs
        string[] prefabPaths = AutoPrefabAssigner.GetAllPrefabPaths();
        availableHumanPrefabs.Clear();
        availableSpiderPrefabs.Clear();
        
        foreach (string path in prefabPaths)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(path).ToLower();
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                if (filename.Contains("human") || filename.Contains("player") || filename.Contains("man"))
                {
                    availableHumanPrefabs.Add(prefab);
                    if (humanPrefab == prefab)
                    {
                        selectedHumanIndex = availableHumanPrefabs.Count - 1;
                    }
                }
                else if (filename.Contains("spider"))
                {
                    availableSpiderPrefabs.Add(prefab);
                    if (spiderPrefab == prefab)
                    {
                        selectedSpiderIndex = availableSpiderPrefabs.Count - 1;
                    }
                }
            }
        }
        
        // If we have human prefabs but no selected one
        if (humanPrefab == null && availableHumanPrefabs.Count > 0)
        {
            selectedHumanIndex = 0;
            humanPrefab = availableHumanPrefabs[0];
        }
        
        // If we have spider prefabs but no selected one
        if (spiderPrefab == null && availableSpiderPrefabs.Count > 0)
        {
            selectedSpiderIndex = 0;
            spiderPrefab = availableSpiderPrefabs[0];
        }
        
        // Scan for rigs whenever prefabs change
        ScanForProceduralRigs();
    }
    
    private void ScanForProceduralRigs()
    {
        availableRigs.Clear();
        customRigs.Clear();
        
        // Check if we have a human prefab to scan
        if (humanPrefab != null)
        {
            // Use PrefabUtility to get prefab contents
            GameObject tempInstance = PrefabUtility.InstantiatePrefab(humanPrefab) as GameObject;
            if (tempInstance != null)
            {
                // Get all transform children and look for those ending with "Rig"
                Transform[] allChildren = tempInstance.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.name.EndsWith("Rig"))
                    {
                        string rigName = child.name;
                        availableRigs.Add(rigName);
                        
                        // Set default state (enabled)
                        if (!customRigs.ContainsKey(rigName))
                        {
                            customRigs[rigName] = true;
                        }
                        
                        // Set default equation solver for this rig
                        if (!rigSolverTypes.ContainsKey(rigName))
                        {
                            rigSolverTypes[rigName] = Modules.Maths.EquationSolverType.SemiImplicitEuler;
                        }
                        
                        // Set default solver parameters for this rig
                        if (!rigSolverParameters.ContainsKey(rigName))
                        {
                            rigSolverParameters[rigName] = new Modules.Maths.EquationSolverParameters(
                                frequency, damping, response, initialPosition);
                        }
                    }
                }
                
                // Clean up the temporary instance
                DestroyImmediate(tempInstance);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Spawner", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Character selection
        EditorGUILayout.LabelField("Select Character Type:", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        selectedCharacterType = (CharacterType)EditorGUILayout.EnumPopup("Character Type", selectedCharacterType);
        if (EditorGUI.EndChangeCheck())
        {
            // If character type changed, re-scan for rigs
            ScanForProceduralRigs();
        }
        
        // Prefab assignments
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefab Settings:", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        
        // Human prefab selection
        GUILayout.BeginHorizontal();
        humanPrefab = (GameObject)EditorGUILayout.ObjectField("Human Prefab", humanPrefab, typeof(GameObject), false);
        
        if (availableHumanPrefabs.Count > 0)
        {
            string[] humanPrefabNames = new string[availableHumanPrefabs.Count];
            for (int i = 0; i < availableHumanPrefabs.Count; i++)
            {
                humanPrefabNames[i] = availableHumanPrefabs[i].name;
            }
            
            int newHumanIndex = EditorGUILayout.Popup(selectedHumanIndex, humanPrefabNames, GUILayout.Width(120));
            
            if (newHumanIndex != selectedHumanIndex)
            {
                selectedHumanIndex = newHumanIndex;
                humanPrefab = availableHumanPrefabs[selectedHumanIndex];
                ScanForProceduralRigs(); // Re-scan for rigs when human prefab changes
            }
        }
        GUILayout.EndHorizontal();
        
        // Spider prefab selection
        GUILayout.BeginHorizontal();
        spiderPrefab = (GameObject)EditorGUILayout.ObjectField("Spider Prefab", spiderPrefab, typeof(GameObject), false);
        
        if (availableSpiderPrefabs.Count > 0)
        {
            string[] spiderPrefabNames = new string[availableSpiderPrefabs.Count];
            for (int i = 0; i < availableSpiderPrefabs.Count; i++)
            {
                spiderPrefabNames[i] = availableSpiderPrefabs[i].name;
            }
            
            int newSpiderIndex = EditorGUILayout.Popup(selectedSpiderIndex, spiderPrefabNames, GUILayout.Width(120));
            
            if (newSpiderIndex != selectedSpiderIndex)
            {
                selectedSpiderIndex = newSpiderIndex;
                spiderPrefab = availableSpiderPrefabs[selectedSpiderIndex];
            }
        }
        GUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            // Update dropdown indices if prefabs changed directly via object field
            UpdateSelectedIndices();
        }
        
        if (GUILayout.Button("Refresh Prefab List"))
        {
            FindPrefabs();
        }
        
        // Procedural Animation Rig Settings (only for Human)
        if (selectedCharacterType == CharacterType.Human)
        {
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Foldout for procedural animation settings
            showProceduralSettings = EditorGUILayout.Foldout(showProceduralSettings, "Procedural Animation Settings", true, EditorStyles.foldoutHeader);
            
            if (showProceduralSettings)
            {
                EditorGUI.indentLevel++;
                
                if (availableRigs.Count == 0)
                {
                    EditorGUILayout.HelpBox("No procedural animation rigs found in the selected human prefab.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.LabelField("Enable/Disable Procedural Animation Rigs:", EditorStyles.boldLabel);
                    
                    // Begin scroll view for rigs
                    rigScrollPosition = EditorGUILayout.BeginScrollView(rigScrollPosition, GUILayout.MaxHeight(150));
                    
                    // Show toggle for each available rig
                    foreach (string rigName in availableRigs)
                    {
                        customRigs[rigName] = EditorGUILayout.Toggle(rigName, customRigs[rigName]);
                    }
                    
                    EditorGUILayout.EndScrollView();
                    
                    if (GUILayout.Button("Toggle All Rigs"))
                    {
                        bool anyEnabled = false;
                        foreach (var rig in customRigs)
                        {
                            if (rig.Value)
                            {
                                anyEnabled = true;
                                break;
                            }
                        }
                        
                        // If any are enabled, disable all; otherwise enable all
                        bool newState = !anyEnabled;
                        foreach (string rigName in availableRigs)
                        {
                            customRigs[rigName] = newState;
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
            
            // Equation Solver Settings
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Foldout for equation solver settings
            showEquationSolverSettings = EditorGUILayout.Foldout(showEquationSolverSettings, "Equation Solver Settings", true, EditorStyles.foldoutHeader);
            
            if (showEquationSolverSettings)
            {
                EditorGUI.indentLevel++;
                
                // Global solver selection
                EditorGUILayout.LabelField("Default Equation Solver:", EditorStyles.boldLabel);
                selectedEquationSolverType = (Modules.Maths.EquationSolverType)EditorGUILayout.EnumPopup("Solver Type", selectedEquationSolverType);
                
                // Solver parameters
                showSolverParameters = EditorGUILayout.Foldout(showSolverParameters, "Solver Parameters", true);
                if (showSolverParameters)
                {
                    EditorGUI.indentLevel++;
                    
                    frequency = EditorGUILayout.Slider("Frequency", frequency, 0.1f, 20f);
                    damping = EditorGUILayout.Slider("Damping", damping, 0f, 2f);
                    response = EditorGUILayout.Slider("Response", response, -3f, 5f);
                    initialPosition = EditorGUILayout.Vector3Field("Initial Position", initialPosition);
                    
                    if (GUILayout.Button("Apply Parameters to All Rigs"))
                    {
                        foreach (string rigName in availableRigs)
                        {
                            rigSolverParameters[rigName] = new Modules.Maths.EquationSolverParameters(
                                frequency, damping, response, initialPosition);
                        }
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                if (GUILayout.Button("Apply Solver Type to All Rigs"))
                {
                    foreach (string rigName in availableRigs)
                    {
                        rigSolverTypes[rigName] = selectedEquationSolverType;
                    }
                }
                
                // Per-rig solver selection
                if (availableRigs.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Per-Rig Equation Solvers:", EditorStyles.boldLabel);
                    
                    foreach (string rigName in availableRigs)
                    {
                        // Only show solver selection for enabled rigs
                        if (customRigs.ContainsKey(rigName) && customRigs[rigName])
                        {
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            
                            EditorGUILayout.LabelField(rigName, EditorStyles.boldLabel);
                            
                            // Solver type selection
                            rigSolverTypes[rigName] = (Modules.Maths.EquationSolverType)EditorGUILayout.EnumPopup(
                                "Solver Type", 
                                rigSolverTypes.ContainsKey(rigName) ? rigSolverTypes[rigName] : selectedEquationSolverType
                            );
                            
                            // Get current parameters
                            Modules.Maths.EquationSolverParameters currentParams = rigSolverParameters.ContainsKey(rigName) 
                                ? rigSolverParameters[rigName] 
                                : new Modules.Maths.EquationSolverParameters(frequency, damping, response, initialPosition);
                            
                            // Parameter fields
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Frequency", GUILayout.Width(80));
                            float newFrequency = EditorGUILayout.Slider(currentParams.Frequency, 0.1f, 20f);
                            EditorGUILayout.EndHorizontal();
                            
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Damping", GUILayout.Width(80));
                            float newDamping = EditorGUILayout.Slider(currentParams.Damping, 0f, 2f);
                            EditorGUILayout.EndHorizontal();
                            
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Response", GUILayout.Width(80));
                            float newResponse = EditorGUILayout.Slider(currentParams.Response, -3, 5f);
                            EditorGUILayout.EndHorizontal();
                            
                            // Update parameters if changed
                            if (newFrequency != currentParams.Frequency || 
                                newDamping != currentParams.Damping || 
                                newResponse != currentParams.Response)
                            {
                                rigSolverParameters[rigName] = new Modules.Maths.EquationSolverParameters(
                                    newFrequency, newDamping, newResponse, currentParams.InitialPosition);
                            }
                            
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }
        else if (selectedCharacterType == CharacterType.Spider)
        {
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Foldout for equation solver settings
            showEquationSolverSettings = EditorGUILayout.Foldout(showEquationSolverSettings, "Spider Movement Settings", true, EditorStyles.foldoutHeader);
            
            if (showEquationSolverSettings)
            {
                EditorGUI.indentLevel++;
                
                // Global solver selection
                EditorGUILayout.LabelField("Equation Solver:", EditorStyles.boldLabel);
                selectedEquationSolverType = (Modules.Maths.EquationSolverType)EditorGUILayout.EnumPopup("Solver Type", selectedEquationSolverType);
                
                // Solver parameters
                showSolverParameters = EditorGUILayout.Foldout(showSolverParameters, "Movement Parameters", true);
                if (showSolverParameters)
                {
                    EditorGUI.indentLevel++;
                    
                    frequency = EditorGUILayout.Slider("Frequency", frequency, 0.1f, 20f);
                    EditorGUILayout.HelpBox("Higher frequency = faster reaction to changes, but may cause jitter", MessageType.Info);
                    
                    damping = EditorGUILayout.Slider("Damping", damping, 0f, 2f);
                    EditorGUILayout.HelpBox("Higher damping = less oscillation/bounce", MessageType.Info);
                    
                    response = EditorGUILayout.Slider("Response", response, -3, 5f);
                    EditorGUILayout.HelpBox("Higher response = stronger reaction to input", MessageType.Info);
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }
        // Spawn settings
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawn Settings:", EditorStyles.boldLabel);
        
        useCustomPosition = EditorGUILayout.Toggle("Use Custom Position", useCustomPosition);
        
        if (useCustomPosition)
        {
            spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", spawnPosition);
        }
        else
        {
            spawnPoint = (Transform)EditorGUILayout.ObjectField("Spawn Point", spawnPoint, typeof(Transform), true);
        }
        
        // Check if prefabs are assigned
        bool canSpawn = (selectedCharacterType == CharacterType.Human && humanPrefab != null) || 
                        (selectedCharacterType == CharacterType.Spider && spiderPrefab != null);

        EditorGUI.BeginDisabledGroup(!canSpawn);
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Spawn Character", GUILayout.Height(30)))
        {
            SpawnCharacter();
        }
        
        EditorGUI.EndDisabledGroup();
        
        if (!canSpawn)
        {
            EditorGUILayout.HelpBox("Please assign the required prefab for the selected character type.", MessageType.Warning);
        }
    }
    
    private void UpdateSelectedIndices()
    {
        // Update human index
        if (humanPrefab != null)
        {
            for (int i = 0; i < availableHumanPrefabs.Count; i++)
            {
                if (availableHumanPrefabs[i] == humanPrefab)
                {
                    selectedHumanIndex = i;
                    break;
                }
            }
        }
        
        // Update spider index
        if (spiderPrefab != null)
        {
            for (int i = 0; i < availableSpiderPrefabs.Count; i++)
            {
                if (availableSpiderPrefabs[i] == spiderPrefab)
                {
                    selectedSpiderIndex = i;
                    break;
                }
            }
        }
    }
    
    private void SpawnCharacter()
    {
        GameObject prefabToSpawn = GetSelectedPrefab();
        
        if (prefabToSpawn == null)
        {
            EditorUtility.DisplayDialog("Error", "No prefab selected to spawn.", "OK");
            return;
        }
        
        // Determine spawn position
        Vector3 position = useCustomPosition ? spawnPosition : (spawnPoint != null ? spawnPoint.position : Vector3.zero);
        
        // Instantiate the character
        GameObject character = PrefabUtility.InstantiatePrefab(prefabToSpawn) as GameObject;
        if (character != null)
        {
            Debug.LogError("Character instantiated");
            // Register for undo
            Undo.RegisterCreatedObjectUndo(character, "Spawn Character");
            
            // Place the character
            character.transform.position = position;
            Debug.LogError("Character positioned");
            
            // Break prefab instance connection for independent editing
            PrefabUtility.UnpackPrefabInstance(character, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            
            // Configure equation solver settings
            ConfigureEquationSolver(character);
            
            // Configure procedural animation rigs
            ConfigureProceduralRigs(character);
            
            // Apply changes to components using SerializedObject for more reliable property setting
            var spiderController = character.GetComponent<SpiderController>();
            if (spiderController != null)
            {
                SerializedObject serializedController = new SerializedObject(spiderController);
                
                // Try to find the properties by name
                SerializedProperty frequencyProp = serializedController.FindProperty("frequency");
                if (frequencyProp != null) frequencyProp.floatValue = frequency;
                
                SerializedProperty dampingProp = serializedController.FindProperty("damping");
                if (dampingProp != null) dampingProp.floatValue = damping;
                
                SerializedProperty responseProp = serializedController.FindProperty("response");
                if (responseProp != null) responseProp.floatValue = response;
                
                // Try to find solverType property and set it by int value
                SerializedProperty solverTypeProp = serializedController.FindProperty("solverType");
                if (solverTypeProp != null) solverTypeProp.enumValueIndex = (int)selectedEquationSolverType;
                
                // Apply all property changes
                serializedController.ApplyModifiedProperties();
                Debug.LogError("Applied serialized properties to SpiderController");
            } 
            // Ensure changes are saved
            EditorUtility.SetDirty(character);
            foreach (var component in character.GetComponentsInChildren<Component>())
            {
                if (component != null)
                {
                    EditorUtility.SetDirty(component);
                }
            }
            
            // Mark scene as dirty
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(character.scene);
            }
            
            Debug.Log($"Spawned {selectedCharacterType} character at {position} with parameters (f={frequency}, d={damping}, r={response})");
            
            // Select the spawned character
            Selection.activeGameObject = character;
        }
        else
        {
            Debug.LogError("Failed to instantiate character prefab.");
        }
    }
    
    private GameObject GetSelectedPrefab()
    {
        return selectedCharacterType == CharacterType.Human ? humanPrefab : spiderPrefab;
    }
    
    private void ConfigureProceduralRigs(GameObject character)
    {
        if (character != null)
        {
            // Get all transform children and configure those that end with "Rig"
            Transform[] allChildren = character.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name.EndsWith("Rig") && customRigs.ContainsKey(child.name))
                {
                    // Enable or disable the rig based on user selection
                    child.gameObject.SetActive(customRigs[child.name]);
                    
                    // Only set solver for enabled rigs
                    if (customRigs[child.name] && rigSolverTypes.ContainsKey(child.name))
                    {
                        SetRigEquationSolver(child.gameObject, rigSolverTypes[child.name]);
                    }
                    
                    if (customRigs[child.name])
                    {
                        Debug.Log($"Enabled procedural animation rig: {child.name}");
                    }
                    else
                    {
                        Debug.Log($"Disabled procedural animation rig: {child.name}");
                    }
                }
            }
            
            // Also set equation solver in MovementContext if it exists
            //SetMovementContextSolver(character, selectedEquationSolverType);
        }
    }
    
    private void ConfigureEquationSolver(GameObject character)
    {
        if (selectedCharacterType == CharacterType.Human)
        {
            Debug.LogError("Human character ConfigureEquationSolver");
            // For human characters, configure equation solver
            ConfigureHumanEquationSolver(character);
        }
        else if (selectedCharacterType == CharacterType.Spider)
        {
            Debug.LogError("Spider character ConfigureEquationSolver");
            // For spider characters, configure equation solver
            ConfigureSpiderEquationSolver(character);
        }
    }
    
    private void ConfigureHumanEquationSolver(GameObject humanObject)
    {
        Debug.LogError("ConfigureHumanEquationSolver");
        if (humanObject != null)
        {
            Debug.LogError("Human is not null");
            // Mark the object for Undo
            Undo.RecordObject(humanObject, "Configure Human Equation Solver");
            
            // Find MovementStateMachine in the human character
            MovementStateMachine movementStateMachine = null;
            
            // Try to find the component directly
            movementStateMachine = humanObject.GetComponent<MovementStateMachine>();
            
            // If not found, try looking in children including inactive ones
            if (movementStateMachine == null)
            {
                movementStateMachine = humanObject.GetComponentInChildren<MovementStateMachine>(true);
                if (movementStateMachine != null)
                {
                    Debug.LogError($"Found MovementStateMachine in child: {movementStateMachine.name}");
                }
            }
            
            // If found, configure it
            if (movementStateMachine != null)
            {
                // Mark the component for Undo
                Undo.RecordObject(movementStateMachine, "Configure MovementStateMachine Parameters");
                
                // Set properties
                movementStateMachine.frequency = frequency;
                movementStateMachine.damping = damping;
                movementStateMachine.response = response;
                
                // Set solver type
                var solverTypeField = movementStateMachine.GetType().GetField("solverType");
                if (solverTypeField != null)
                {
                    // Convert enum to int value first
                    int enumValue = (int)selectedEquationSolverType;
                    
                    // Create enum value of the target type
                    object enumObject = Enum.ToObject(solverTypeField.FieldType, enumValue);
                    
                    // Set the field value
                    solverTypeField.SetValue(movementStateMachine, enumObject);
                    Debug.LogError($"Set movementStateMachine solverType to {enumValue} via reflection");
                }
                
                // Try to invoke initialize method if it exists
                var initMethod = movementStateMachine.GetType().GetMethod("InitializeEquationSolver", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                if (initMethod != null)
                {
                    initMethod.Invoke(movementStateMachine, null);
                    Debug.LogError("Called InitializeEquationSolver on MovementStateMachine");
                }
                
                EditorUtility.SetDirty(movementStateMachine);
                Debug.Log($"Configured equation solver on MovementStateMachine with parameters " +
                    $"(f={frequency}, d={damping}, r={response})");
            }
            else
            {
                Debug.LogError("Could not find MovementStateMachine component on human character");
            }
            
            // Mark scene as dirty to ensure changes are saved
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(humanObject.scene);
            }
        }
    }
    
    private void ConfigureSpiderEquationSolver(GameObject spiderObject)
    {
        Debug.LogError("ConfigureSpiderEquationSolver");
        if (spiderObject != null)
        {
            Debug.LogError("Spider is not null");
            // Mark the object for Undo
            Undo.RecordObject(spiderObject, "Configure Equation Solver");
            
            // Set solver properties directly on the SpiderController
            var spiderController = spiderObject.GetComponent<SpiderController>();
            if (spiderController != null)
            {
                Debug.LogError("SpiderController found");
                // Mark the component for Undo
                Undo.RecordObject(spiderController, "Configure SpiderController Parameters");
                
                // Handle potential namespace difference by using reflection for all cases
                var solverTypeField = spiderController.GetType().GetField("solverType");
                if (solverTypeField != null)
                {
                    // Convert enum to int value first
                    int enumValue = (int)selectedEquationSolverType;
                    
                    // Create enum value of the target type
                    object enumObject = Enum.ToObject(solverTypeField.FieldType, enumValue);
                    
                    // Set the field value
                    solverTypeField.SetValue(spiderController, enumObject);
                    Debug.LogError($"Set solverType to {enumValue} via reflection");
                }
                else
                {
                    Debug.LogError("Could not find solverType field on SpiderController");
                }
                
                // Set the other parameters directly
                spiderController.frequency = frequency;
                spiderController.damping = damping;
                spiderController.response = response;
                
                // Force reinitialize the equation solver with the new settings
                if (Application.isPlaying)
                {
                    // If playing, call the public method
                    spiderController.InitializeEquationSolver();
                }
                else
                {
                    // In editor, we need to mark this as dirty to save changes
                    EditorUtility.SetDirty(spiderController);
                    
                    // Call InitializeEquationSolver
                    var method = spiderController.GetType().GetMethod("InitializeEquationSolver", 
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    if (method != null)
                    {
                        method.Invoke(spiderController, null);
                    }
                }
                
                Debug.Log($"Configured equation solver on SpiderController with parameters " +
                    $"(f={frequency}, d={damping}, r={response})");
            }
            
            // Also set it on the MovementStateMachine if it exists
            MovementStateMachine movementStateMachine = null;
            
            // Try to find the component directly
            movementStateMachine = spiderObject.GetComponent<MovementStateMachine>();
            
            // If not found, try looking in children including inactive ones
            if (movementStateMachine == null)
            {
                movementStateMachine = spiderObject.GetComponentInChildren<MovementStateMachine>(true);
                if (movementStateMachine != null)
                {
                    Debug.LogError($"Found MovementStateMachine in child: {movementStateMachine.name}");
                }
            }
            
            // If still not found, try to find the MovementRig GameObject and get the component from there
            if (movementStateMachine == null)
            {
                Transform movementRig = spiderObject.transform.Find("MovementRig");
                if (movementRig != null)
                {
                    Debug.LogError($"Found MovementRig GameObject: {movementRig.name}");
                    movementStateMachine = movementRig.GetComponent<MovementStateMachine>();
                    if (movementStateMachine == null)
                    {
                        // Try looking in the children of the MovementRig
                        movementStateMachine = movementRig.GetComponentInChildren<MovementStateMachine>(true);
                    }
                }
            }
            
            // Search all children recursively for "LegMovementController" as well
            Component legMovementController = null;
            var transforms = spiderObject.GetComponentsInChildren<Transform>(true);
            foreach (var transform in transforms)
            {
                var component = transform.GetComponent("LegMovementController");
                if (component != null)
                {
                    legMovementController = component;
                    Debug.LogError($"Found LegMovementController on: {transform.name}");
                }
            }
            
            // Apply settings to LegMovementController if found
            if (legMovementController != null)
            {
                Undo.RecordObject(legMovementController, "Configure LegMovementController Parameters");
                
                // Set parameters using reflection since we might not have the type
                SetFieldOnComponent(legMovementController, "frequency", frequency);
                SetFieldOnComponent(legMovementController, "damping", damping);
                SetFieldOnComponent(legMovementController, "response", response);
                
                // Try to invoke initialize method if it exists
                var initMethod = legMovementController.GetType().GetMethod("InitializeEquationSolver", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                if (initMethod != null)
                {
                    initMethod.Invoke(legMovementController, null);
                    Debug.LogError("Called InitializeEquationSolver on LegMovementController");
                }
                
                EditorUtility.SetDirty(legMovementController);
            }
            
            // Mark scene as dirty to ensure changes are saved
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(spiderObject.scene);
            }
        }
    }
    
    private void SetRigEquationSolver(GameObject rig, Modules.Maths.EquationSolverType solverType)
    {
        // Here we use reflection to find components that might use an equation solver
        // Look for components that might have equation solver fields
        Component[] components = rig.GetComponents<Component>();
        string rigName = rig.name;
        
        // Get parameters for this rig
        Modules.Maths.EquationSolverParameters parameters = rigSolverParameters.ContainsKey(rigName)
            ? rigSolverParameters[rigName]
            : new Modules.Maths.EquationSolverParameters(frequency, damping, response, initialPosition);
        
        bool solverSet = false;
        
        foreach (Component component in components)
        {
            if (component == null) continue;
            
            System.Type type = component.GetType();
            
            // Check for fields that might be equation solvers
            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                if (field.FieldType == typeof(IEquationSolver))
                {
                    field.SetValue(component, Modules.Maths.EquationSolverFactory.CreateSolver(solverType, parameters));
                    Debug.Log($"Set {solverType} equation solver on {component.GetType().Name}.{field.Name} with parameters " +
                        $"(f={parameters.Frequency}, d={parameters.Damping}, r={parameters.Response})");
                    solverSet = true;
                }
            }
        }
        
        if (!solverSet)
        {
            // If we couldn't find a direct field, look for methods that might set the solver
            foreach (Component component in components)
            {
                if (component == null) continue;
                
                System.Type type = component.GetType();
                
                foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if ((method.Name.Contains("SetSolver") || method.Name.Contains("SetEquationSolver")) && 
                        method.GetParameters().Length == 1 && 
                        method.GetParameters()[0].ParameterType == typeof(IEquationSolver))
                    {
                        method.Invoke(component, new object[] { Modules.Maths.EquationSolverFactory.CreateSolver(solverType, parameters) });
                        Debug.Log($"Set {solverType} equation solver using {component.GetType().Name}.{method.Name} with parameters " +
                            $"(f={parameters.Frequency}, d={parameters.Damping}, r={parameters.Response})");
                        solverSet = true;
                    }
                }
            }
        }
    }
    
    
    private bool SetFieldOnComponent(Component component, string fieldName, object value)
    {
        if (component == null) return false;
        
        System.Type type = component.GetType();
        
        try
        {
            // Try to find field with exact name first
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null && field.FieldType.IsAssignableFrom(value.GetType()))
            {
                field.SetValue(component, value);
                return true;
            }
            
            // If not found, try to find fields that match the expected type
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var f in fields)
            {
                if (f.FieldType.IsAssignableFrom(value.GetType()) && 
                    (f.Name.ToLower().Contains("solver") || f.Name.ToLower().Contains("equation")))
                {
                    f.SetValue(component, value);
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting field on {type.Name}: {e.Message}");
        }
        
        return false;
    }
    
   
    
        
   
} 
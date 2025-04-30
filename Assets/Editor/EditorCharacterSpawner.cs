using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EditorCharacterSpawner : EditorWindow
{
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

    [MenuItem("Tools/Character Spawner")]
    public static void ShowWindow()
    {
        EditorCharacterSpawner window = GetWindow<EditorCharacterSpawner>("Character Spawner");
        window.minSize = new Vector2(300, 250);
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
        spiderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TestSpiderOrange.prefab");
        
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
        // Get the prefab to spawn
        GameObject prefabToSpawn = selectedCharacterType == CharacterType.Human ? humanPrefab : spiderPrefab;
        
        if (prefabToSpawn == null)
        {
            Debug.LogError($"Prefab for {selectedCharacterType} is not assigned!");
            return;
        }
        
        // Determine spawn position
        Vector3 position;
        if (useCustomPosition)
        {
            position = spawnPosition;
        }
        else if (spawnPoint != null)
        {
            position = spawnPoint.position;
        }
        else
        {
            // Use scene view camera position if no spawn point is set
            if (SceneView.lastActiveSceneView != null)
            {
                position = SceneView.lastActiveSceneView.camera.transform.position + 
                          SceneView.lastActiveSceneView.camera.transform.forward * 2f;
                position.y = 0.5f; // Place at a reasonable height
            }
            else
            {
                position = spawnPosition;
            }
        }
        
        // Create the object
        GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
        Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Character");
        
        // Position the object
        spawnedObject.transform.position = position;
        spawnedObject.transform.rotation = Quaternion.identity;
        
        // For human characters, handle the procedural animation rigs
        if (selectedCharacterType == CharacterType.Human)
        {
            ConfigureProceduralRigs(spawnedObject);
        }
        
        // Select the spawned object
        Selection.activeGameObject = spawnedObject;
        
        // Focus the scene view on the spawned object
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        Debug.Log($"Spawned {selectedCharacterType} character at {position}");
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
        }
    }
} 
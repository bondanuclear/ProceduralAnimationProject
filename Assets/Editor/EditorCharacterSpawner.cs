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

    [MenuItem("Tools/Character Spawner")]
    public static void ShowWindow()
    {
        EditorCharacterSpawner window = GetWindow<EditorCharacterSpawner>("Character Spawner");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        // Try to find prefabs automatically
        FindPrefabs();
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
                if (filename.Contains("human") || filename.Contains("player"))
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
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Spawner", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Character selection
        EditorGUILayout.LabelField("Select Character Type:", EditorStyles.boldLabel);
        selectedCharacterType = (CharacterType)EditorGUILayout.EnumPopup("Character Type", selectedCharacterType);
        
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
        
        // Select the spawned object
        Selection.activeGameObject = spawnedObject;
        
        // Focus the scene view on the spawned object
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        Debug.Log($"Spawned {selectedCharacterType} character at {position}");
    }
} 
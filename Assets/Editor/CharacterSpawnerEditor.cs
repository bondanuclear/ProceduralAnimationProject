using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterSpawner))]
public class CharacterSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        CharacterSpawner spawner = (CharacterSpawner)target;
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Auto-Assign Prefabs"))
        {
            AssignPrefabs(spawner);
        }
    }
    
    private void AssignPrefabs(CharacterSpawner spawner)
    {
        SerializedProperty humanProperty = serializedObject.FindProperty("humanPrefab");
        SerializedProperty spiderProperty = serializedObject.FindProperty("spiderPrefab");
        
        // Find prefabs by name
        GameObject humanPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Player.prefab");
        GameObject spiderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TestSpiderOrange.prefab");
        
        if (humanPrefab != null)
        {
            humanProperty.objectReferenceValue = humanPrefab;
            Debug.Log("Human prefab assigned");
        }
        else
        {
            Debug.LogWarning("Human prefab not found");
        }
        
        if (spiderPrefab != null)
        {
            spiderProperty.objectReferenceValue = spiderPrefab;
            Debug.Log("Spider prefab assigned");
        }
        else
        {
            Debug.LogWarning("Spider prefab not found");
        }
        
        serializedObject.ApplyModifiedProperties();
    }
} 
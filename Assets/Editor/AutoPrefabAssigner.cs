using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class AutoPrefabAssigner
{
    /// <summary>
    /// Finds all prefabs of a specific type in the project
    /// </summary>
    public static List<GameObject> FindAllPrefabsOfType(System.Type componentType)
    {
        List<GameObject> results = new List<GameObject>();
        string[] prefabPaths = GetAllPrefabPaths();
        
        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && prefab.GetComponent(componentType) != null)
            {
                results.Add(prefab);
            }
        }
        
        return results;
    }
    
    /// <summary>
    /// Gets a list of all prefab paths in the project
    /// </summary>
    public static string[] GetAllPrefabPaths()
    {
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        List<string> prefabs = new List<string>();
        
        foreach (string path in allAssets)
        {
            if (path.EndsWith(".prefab") && path.StartsWith("Assets/"))
            {
                prefabs.Add(path);
            }
        }
        
        return prefabs.ToArray();
    }
    
    /// <summary>
    /// Searches for prefabs with specific names
    /// </summary>
    public static Dictionary<string, GameObject> FindPrefabsByNames(params string[] prefabNames)
    {
        Dictionary<string, GameObject> results = new Dictionary<string, GameObject>();
        
        foreach (string name in prefabNames)
        {
            results[name] = null;
        }
        
        string[] prefabPaths = GetAllPrefabPaths();
        
        foreach (string path in prefabPaths)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            
            foreach (string name in prefabNames)
            {
                if (filename.Contains(name) && results[name] == null)
                {
                    results[name] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    break;
                }
            }
        }
        
        return results;
    }
} 
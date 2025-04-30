using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private GameObject humanPrefab;
    [SerializeField] private GameObject spiderPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnHeight = 0.5f;
    
    private CharacterType selectedCharacterType = CharacterType.Human;
    private GameObject currentCharacter;
    
    private void OnGUI()
    {
        // Create a GUI box at the top of the screen
        GUI.Box(new Rect(10, 10, 200, 120), "Character Selection");
        
        // Radio button for Human
        bool isHumanSelected = selectedCharacterType == CharacterType.Human;
        bool newHumanSelection = GUI.Toggle(new Rect(20, 40, 100, 20), isHumanSelected, "Human", "Radio");
        
        // Radio button for Spider
        bool isSpiderSelected = selectedCharacterType == CharacterType.Spider;
        bool newSpiderSelection = GUI.Toggle(new Rect(20, 70, 100, 20), isSpiderSelected, "Spider", "Radio");
        
        // Update the selected character type based on the toggle selection
        if (newHumanSelection && !isHumanSelected)
        {
            selectedCharacterType = CharacterType.Human;
        }
        else if (newSpiderSelection && !isSpiderSelected)
        {
            selectedCharacterType = CharacterType.Spider;
        }
        
        // Spawn button
        if (GUI.Button(new Rect(20, 100, 100, 20), "Spawn"))
        {
            SpawnCharacter();
        }
    }
    
    private void SpawnCharacter()
    {
        // Destroy the current character if one exists
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }
        
        // Spawn the selected character
        Vector3 position = spawnPoint != null 
            ? spawnPoint.position 
            : new Vector3(0, spawnHeight, 0);
            
        GameObject prefabToSpawn = selectedCharacterType == CharacterType.Human 
            ? humanPrefab 
            : spiderPrefab;
            
        if (prefabToSpawn != null)
        {
            currentCharacter = Instantiate(prefabToSpawn, position, Quaternion.identity);
            Debug.Log($"Spawned {selectedCharacterType} character");
        }
        else
        {
            Debug.LogError($"Prefab for {selectedCharacterType} is not assigned!");
        }
    }
} 
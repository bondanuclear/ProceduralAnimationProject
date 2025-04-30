using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpawnerUI : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private GameObject humanPrefab;
    [SerializeField] private GameObject spiderPrefab;
    
    [Header("UI References")]
    [SerializeField] private Toggle humanToggle;
    [SerializeField] private Toggle spiderToggle;
    [SerializeField] private Button spawnButton;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnHeight = 0.5f;
    
    private CharacterType selectedCharacterType = CharacterType.Human;
    private GameObject currentCharacter;
    
    private void Start()
    {
        // Set up the UI event handlers
        if (humanToggle != null)
        {
            humanToggle.onValueChanged.AddListener(isOn => 
            { 
                if (isOn) selectedCharacterType = CharacterType.Human; 
            });
        }
        
        if (spiderToggle != null)
        {
            spiderToggle.onValueChanged.AddListener(isOn => 
            { 
                if (isOn) selectedCharacterType = CharacterType.Spider; 
            });
        }
        
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnCharacter);
        }
        else
        {
            Debug.LogError("Spawn button reference not set!");
        }
        
        // Set the default toggle
        if (humanToggle != null)
        {
            humanToggle.isOn = true;
        }
    }
    
    public void SpawnCharacter()
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
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterSpawnerUI))]
public class CharacterSpawnerUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        CharacterSpawnerUI spawner = (CharacterSpawnerUI)target;
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Auto-Assign Prefabs"))
        {
            AssignPrefabs(spawner);
        }
        
        if (GUILayout.Button("Create UI Elements"))
        {
            CreateUIElements(spawner);
        }
    }
    
    private void AssignPrefabs(CharacterSpawnerUI spawner)
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
    
    private void CreateUIElements(CharacterSpawnerUI spawner)
    {
        GameObject canvasObj = null;
        
        // Check if a canvas already exists in the scene
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvasObj = canvas.gameObject;
                break;
            }
        }
        
        // If no canvas exists, create one
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Character Selection Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
        // Create a panel for the character selection UI
        GameObject panelObj = new GameObject("Character Selection Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchoredPosition = new Vector2(10, -10);
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.sizeDelta = new Vector2(200, 150);
        
        UnityEngine.UI.Image panelImage = panelObj.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create a title for the panel
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, -15);
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 30);
        
        UnityEngine.UI.Text titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
        titleText.text = "Character Selection";
        titleText.fontSize = 16;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        // Create a toggle group
        GameObject toggleGroupObj = new GameObject("Toggle Group");
        toggleGroupObj.transform.SetParent(panelObj.transform, false);
        UnityEngine.UI.ToggleGroup toggleGroup = toggleGroupObj.AddComponent<UnityEngine.UI.ToggleGroup>();
        
        RectTransform toggleGroupRect = toggleGroupObj.GetComponent<RectTransform>();
        toggleGroupRect.anchoredPosition = new Vector2(0, -60);
        toggleGroupRect.anchorMin = new Vector2(0, 1);
        toggleGroupRect.anchorMax = new Vector2(1, 1);
        toggleGroupRect.sizeDelta = new Vector2(0, 60);
        
        // Create Human toggle
        GameObject humanToggleObj = CreateToggle("Human Toggle", toggleGroupObj.transform, new Vector2(0, -15), "Human");
        UnityEngine.UI.Toggle humanToggle = humanToggleObj.GetComponent<UnityEngine.UI.Toggle>();
        humanToggle.group = toggleGroup;
        humanToggle.isOn = true;
        
        // Create Spider toggle
        GameObject spiderToggleObj = CreateToggle("Spider Toggle", toggleGroupObj.transform, new Vector2(0, -45), "Spider");
        UnityEngine.UI.Toggle spiderToggle = spiderToggleObj.GetComponent<UnityEngine.UI.Toggle>();
        spiderToggle.group = toggleGroup;
        
        // Create Spawn button
        GameObject buttonObj = new GameObject("Spawn Button");
        buttonObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchoredPosition = new Vector2(0, -125);
        buttonRect.anchorMin = new Vector2(0.5f, 1);
        buttonRect.anchorMax = new Vector2(0.5f, 1);
        buttonRect.sizeDelta = new Vector2(120, 30);
        
        UnityEngine.UI.Image buttonImage = buttonObj.AddComponent<UnityEngine.UI.Image>();
        buttonImage.color = new Color(0.3f, 0.6f, 0.9f, 1);
        
        UnityEngine.UI.Button button = buttonObj.AddComponent<UnityEngine.UI.Button>();
        
        GameObject buttonTextObj = new GameObject("Button Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Text buttonText = buttonTextObj.AddComponent<UnityEngine.UI.Text>();
        buttonText.text = "Spawn";
        buttonText.fontSize = 14;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        
        // Assign references in the CharacterSpawnerUI component
        SerializedProperty humanToggleProperty = serializedObject.FindProperty("humanToggle");
        SerializedProperty spiderToggleProperty = serializedObject.FindProperty("spiderToggle");
        SerializedProperty spawnButtonProperty = serializedObject.FindProperty("spawnButton");
        
        humanToggleProperty.objectReferenceValue = humanToggle;
        spiderToggleProperty.objectReferenceValue = spiderToggle;
        spawnButtonProperty.objectReferenceValue = button;
        
        serializedObject.ApplyModifiedProperties();
        
        Debug.Log("UI elements created and assigned");
    }
    
    private GameObject CreateToggle(string name, Transform parent, Vector2 position, string label)
    {
        GameObject toggleObj = new GameObject(name);
        toggleObj.transform.SetParent(parent, false);
        
        RectTransform toggleRect = toggleObj.AddComponent<RectTransform>();
        toggleRect.anchoredPosition = position;
        toggleRect.anchorMin = new Vector2(0.5f, 1);
        toggleRect.anchorMax = new Vector2(0.5f, 1);
        toggleRect.sizeDelta = new Vector2(120, 20);
        
        UnityEngine.UI.Toggle toggle = toggleObj.AddComponent<UnityEngine.UI.Toggle>();
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(toggleObj.transform, false);
        
        RectTransform backgroundRect = background.AddComponent<RectTransform>();
        backgroundRect.anchoredPosition = new Vector2(-50, 0);
        backgroundRect.anchorMin = new Vector2(0, 0.5f);
        backgroundRect.anchorMax = new Vector2(0, 0.5f);
        backgroundRect.sizeDelta = new Vector2(20, 20);
        
        UnityEngine.UI.Image backgroundImage = background.AddComponent<UnityEngine.UI.Image>();
        backgroundImage.color = Color.white;
        
        // Create checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        
        RectTransform checkmarkRect = checkmark.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkmarkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkmarkRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Image checkmarkImage = checkmark.AddComponent<UnityEngine.UI.Image>();
        checkmarkImage.color = new Color(0.2f, 0.6f, 1f);
        
        // Create label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(toggleObj.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(10, 0);
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(20, 0);
        
        UnityEngine.UI.Text labelText = labelObj.AddComponent<UnityEngine.UI.Text>();
        labelText.text = label;
        labelText.fontSize = 14;
        labelText.alignment = TextAnchor.MiddleLeft;
        
        // Hook up the toggle component
        toggle.targetGraphic = backgroundImage;
        toggle.graphic = checkmarkImage;
        
        return toggleObj;
    }
} 
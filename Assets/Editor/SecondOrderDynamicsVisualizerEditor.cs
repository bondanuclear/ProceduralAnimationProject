using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SecondOrderDynamicsVisualizer))]
public class SecondOrderDynamicsVisualizerEditor : Editor
{
    private const int graphHeight = 140;
    private const int graphWidth = 300;
    
    public override void OnInspectorGUI()
    {
        SecondOrderDynamicsVisualizer visualizer = (SecondOrderDynamicsVisualizer)target;
        
        // Check for parameter changes
        EditorGUI.BeginChangeCheck();
        
        // Group parameters in a nice UI
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        GUILayout.Label("Script", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), GUIContent.none);
        
        EditorGUILayout.Space(5);
        
        GUILayout.Label("Update Mode", EditorStyles.boldLabel);
        
        // Update mode dropdown (purely visual, as we don't use it here)
        string[] updateModes = { "Update" };
        EditorGUILayout.Popup(0, updateModes);
        
        // Parameters with sliders
        EditorGUILayout.Space(5);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        float newF = EditorGUILayout.Slider("F", visualizer.f, 0.1f, 10f);
        if (newF != visualizer.f)
        {
            Undo.RecordObject(visualizer, "Change F Parameter");
            visualizer.f = newF;
        }
        
        float newZ = EditorGUILayout.Slider("Z", visualizer.z, 0f, 2f);
        if (newZ != visualizer.z)
        {
            Undo.RecordObject(visualizer, "Change Z Parameter");
            visualizer.z = newZ;
        }
        
        float newR = EditorGUILayout.Slider("R", visualizer.r, -5f, 5f);
        if (newR != visualizer.r)
        {
            Undo.RecordObject(visualizer, "Change R Parameter");
            visualizer.r = newR;
        }
        
        EditorGUILayout.EndVertical();
        
        // Solver selection
        EditorGUILayout.PropertyField(serializedObject.FindProperty("solverType"), new GUIContent("Solver Type"));
        
        EditorGUILayout.Space(5);
        
        // Target selection field
        EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("X (Transform)", EditorStyles.miniLabel);
        
        EditorGUILayout.EndVertical();
        
        // Check if parameters have changed
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            visualizer.RunSimulation();
        }
        
        EditorGUILayout.Space(10);
        
        // Draw graph
        GUILayout.Label("X (Transform) Visualization", EditorStyles.boldLabel);
        
        Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
        visualizer.graphRect = rect;
        
        // Draw graph background and grid
        EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));
        
        Handles.BeginGUI();
        
        // Draw centerline
        Handles.color = Color.green;
        float centerY = rect.y + rect.height / 2;
        Handles.DrawLine(
            new Vector3(rect.x, centerY), 
            new Vector3(rect.x + rect.width, centerY)
        );
        
        // Get visualization data
        List<float> data = visualizer.GetSimulationData();
        
        // Draw the curve
        if (data.Count >= 2)
        {
            // Find min/max for scaling
            float min = float.MaxValue;
            float max = float.MinValue;
            
            foreach (float value in data)
            {
                min = Mathf.Min(min, value);
                max = Mathf.Max(max, value);
            }
            
            // Ensure we have a reasonable range
            float range = Mathf.Max(0.01f, max - min);
            
            // Draw line segments
            Handles.color = Color.cyan;
            
            for (int i = 1; i < data.Count; i++)
            {
                float x0 = rect.x + ((i - 1) * rect.width / (data.Count - 1));
                float y0 = rect.yMax - ((data[i - 1] - min) / range) * rect.height;
                
                float x1 = rect.x + (i * rect.width / (data.Count - 1));
                float y1 = rect.yMax - ((data[i] - min) / range) * rect.height;
                
                Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
            }
        }
        
        Handles.EndGUI();
        
        // Show values
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"f = {visualizer.f:F2}", EditorStyles.miniLabel, GUILayout.Width(70));
        EditorGUILayout.LabelField($"z = {visualizer.z:F2}", EditorStyles.miniLabel, GUILayout.Width(70));
        EditorGUILayout.LabelField($"r = {visualizer.r:F2}", EditorStyles.miniLabel);
        EditorGUILayout.EndHorizontal();
    }
} 
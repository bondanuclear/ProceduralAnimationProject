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
        
        GUILayout.Label("Simulation Settings", EditorStyles.boldLabel);
        
        // Time step mode selection
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useFixedDeltaTime"), new GUIContent("Use Fixed Delta Time"));
        
        // Show time step information
        float timeStep = visualizer.useFixedDeltaTime ? Time.fixedDeltaTime : visualizer.duration / visualizer.resolution;
        int steps = visualizer.useFixedDeltaTime ? Mathf.FloorToInt(visualizer.duration / Time.fixedDeltaTime) : visualizer.resolution;
        
        EditorGUILayout.LabelField("Time Step: " + timeStep.ToString("F5") + "s", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("Simulation Steps: " + steps, EditorStyles.miniLabel);
        
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
        
        // Get visualization data
        List<float> data = visualizer.GetSimulationData();
        
        // Find min/max for scaling
        float min = float.MaxValue;
        float max = float.MinValue;
        
        if (data.Count >= 2)
        {
            foreach (float value in data)
            {
                min = Mathf.Min(min, value);
                max = Mathf.Max(max, value);
            }
        }
        else
        {
            min = 0;
            max = 1;
        }
        
        // Ensure we have a reasonable range
        float range = Mathf.Max(0.01f, max - min);
        
        // Calculate where 0 and 1 will be on the graph
        float zeroY = rect.yMax - ((0 - min) / range) * rect.height;
        float oneY = rect.yMax - ((1 - min) / range) * rect.height;
        
        // Draw X axis (time)
        Handles.color = Color.white;
        Handles.DrawLine(
            new Vector3(rect.x, rect.yMax), 
            new Vector3(rect.x + rect.width, rect.yMax),
            2
        );
        
        // Draw Y axis
        Handles.DrawLine(
            new Vector3(rect.x, rect.y), 
            new Vector3(rect.x, rect.yMax),
            2
        );
        
        // // Draw zero line (reference)
        // Handles.color = Color.white;
        // Handles.DrawLine(
        //     new Vector3(rect.x, zeroY), 
        //     new Vector3(rect.x + rect.width, zeroY),
        //     2
        // );
        
        // Draw equilibrium line (target position = 1)
        Handles.color = Color.green;
        Handles.DrawLine(
            new Vector3(rect.x, oneY), 
            new Vector3(rect.x + rect.width, oneY)
        );
        
        // Draw labels
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 10;
        
        // Y-axis labels
        GUI.Label(new Rect(rect.x - 15, zeroY - 8, 20, 16), "0", labelStyle);
        GUI.Label(new Rect(rect.x - 15, oneY - 8, 20, 16), "1", labelStyle);
        
        // X-axis labels
        //GUI.Label(new Rect(rect.x - 5, rect.yMax, 20, 16), "0", labelStyle);
        //GUI.Label(new Rect(rect.x + rect.width - 15, rect.yMax, 20, 16), "2", labelStyle);
        
        // Draw the curve
        if (data.Count >= 2)
        {
            // Draw line segments
            Handles.color = Color.cyan;
            
            // Create points array for the polyline
            Vector3[] points = new Vector3[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                float x = rect.x + (i * rect.width / (data.Count - 1));
                float y = rect.yMax - ((data[i] - min) / range) * rect.height;
                points[i] = new Vector3(x, y, 0);
            }
            
            // Draw a thicker line using DrawAAPolyLine
            Handles.DrawAAPolyLine(2.0f, points);
            
            // Old individual line drawing approach
            /*
            for (int i = 1; i < data.Count; i++)
            {
                float x0 = rect.x + ((i - 1) * rect.width / (data.Count - 1));
                float y0 = rect.yMax - ((data[i - 1] - min) / range) * rect.height;
                
                float x1 = rect.x + (i * rect.width / (data.Count - 1));
                float y1 = rect.yMax - ((data[i] - min) / range) * rect.height;
                
                Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
            }
            */
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
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(SecondOrderTheoretical))]
public class SecondOrderTheoreticalEditor : Editor
{
    private const int graphHeight = 200;
    private const int graphWidth = 400;
    private bool showParameterPresets = false;
    
    // Parameter presets
    private static readonly (string name, float f, float z, float r)[] presets = new[]
    {
        ("Critically Damped", 2.0f, 1.0f, 1.0f),
        ("Underdamped (Oscillating)", 2.0f, 0.3f, 1.0f),
        ("Overdamped (Smooth)", 2.0f, 1.5f, 1.0f),
        ("Fast Response", 5.0f, 0.7f, 1.0f),
        ("Slow Response", 1.0f, 0.5f, 1.0f),
        ("Bouncy", 3.0f, 0.1f, 1.0f)
    };

    public override void OnInspectorGUI()
    {
        SecondOrderTheoretical theoretical = (SecondOrderTheoretical)target;
        
        // Check for parameter changes
        EditorGUI.BeginChangeCheck();
        
        // Draw default inspector with parameters
        DrawDefaultInspector();
        
        bool changed = EditorGUI.EndChangeCheck();
        if (changed)
        {
            // If parameters were changed, recalculate the response curve
            theoretical.CalculateResponseCurve();
        }
        
        EditorGUILayout.Space(10);
        
        // Parameter presets section
        showParameterPresets = EditorGUILayout.Foldout(showParameterPresets, "Parameter Presets", true, EditorStyles.foldoutHeader);
        if (showParameterPresets)
        {
            EditorGUILayout.LabelField("Select a preset to see different responses:", EditorStyles.wordWrappedLabel);
            
            foreach (var preset in presets)
            {
                if (GUILayout.Button(preset.name))
                {
                    Undo.RecordObject(theoretical, "Apply Parameter Preset");
                    
                    theoretical.frequency = preset.f;
                    theoretical.damping = preset.z;
                    theoretical.response = preset.r;
                    
                    // Recalculate with new parameters
                    theoretical.CalculateResponseCurve();
                }
            }
        }
        
        EditorGUILayout.Space(10);
        
        // Response visualization
        EditorGUILayout.LabelField("Step Response Visualization", EditorStyles.boldLabel);
        
        // Draw the graph
        Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
        DrawResponseGraph(rect, theoretical.GetResponseCurve());
        
        // Draw info about current system type
        string systemType = GetSystemType(theoretical.damping);
        EditorGUILayout.LabelField($"System Type: {systemType}", EditorStyles.boldLabel);
        
        // Add explanation for parameters
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Parameter Explanation:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("• Frequency (f): Higher values = faster oscillations", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("• Damping (z): Controls oscillation decay. z<1: oscillating, z=1: critical, z>1: no oscillation", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("• Response (r): Amplifies or reduces the response magnitude", EditorStyles.wordWrappedLabel);
    }

    private void DrawResponseGraph(Rect rect, List<Vector2> data)
    {
        if (data == null || data.Count < 2) return;

        // Draw graph background
        EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));
        
        Handles.BeginGUI();
        
        // Draw axes
        Handles.color = new Color(0.3f, 0.3f, 0.3f);
        
        // Y-axis
        Handles.DrawLine(
            new Vector3(rect.x, rect.y),
            new Vector3(rect.x, rect.y + rect.height)
        );
        
        // X-axis (at y=0.5)
        Handles.color = new Color(0.3f, 0.3f, 0.3f);
        float centerY = rect.y + rect.height / 2;
        Handles.DrawLine(
            new Vector3(rect.x, centerY),
            new Vector3(rect.x + rect.width, centerY)
        );
        
        // Draw grid lines
        Handles.color = new Color(0.2f, 0.2f, 0.2f);
        for (int i = 1; i < 10; i++)
        {
            float y = rect.y + (i * rect.height / 10);
            Handles.DrawLine(
                new Vector3(rect.x, y),
                new Vector3(rect.x + rect.width, y)
            );
            
            float x = rect.x + (i * rect.width / 10);
            Handles.DrawLine(
                new Vector3(x, rect.y),
                new Vector3(x, rect.y + rect.height)
            );
        }
        
        // Find max for scaling
        float maxY = 0;
        foreach (Vector2 point in data)
        {
            maxY = Mathf.Max(maxY, Mathf.Abs(point.y));
        }
        maxY = Mathf.Max(maxY, 1.0f); // Ensure min scale
        
        // Calculate scale
        float timeScale = rect.width / data[data.Count - 1].x;
        float yScale = (rect.height / 2) / maxY;
        
        // Draw curve
        Handles.color = Color.cyan;
        for (int i = 1; i < data.Count; i++)
        {
            float x0 = rect.x + data[i - 1].x * timeScale;
            float y0 = centerY - data[i - 1].y * yScale;
            
            float x1 = rect.x + data[i].x * timeScale;
            float y1 = centerY - data[i].y * yScale;
            
            Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
        }
        
        // Draw target line (y = 1.0)
        Handles.color = Color.green;
        float targetY = centerY - 1.0f * yScale;
        Handles.DrawLine(
            new Vector3(rect.x, targetY),
            new Vector3(rect.x + rect.width, targetY)
        );
        
        Handles.EndGUI();
        
        // Draw legend
        float padding = 10;
        Rect legendRect = new Rect(rect.x + padding, rect.y + padding, 100, 50);
        GUI.Box(legendRect, "", EditorStyles.helpBox);
        
        Rect labelRect = new Rect(legendRect.x + 5, legendRect.y + 5, legendRect.width - 10, 20);
        GUI.Label(labelRect, "Response", EditorStyles.boldLabel);
        
        Rect lineRect = new Rect(labelRect.x, labelRect.y + 20, 20, 20);
        EditorGUI.DrawRect(new Rect(lineRect.x, lineRect.y + 8, lineRect.width, 4), Color.cyan);
        
        Rect textRect = new Rect(lineRect.x + lineRect.width + 5, lineRect.y, legendRect.width - lineRect.width - 10, 20);
        GUI.Label(textRect, "x(t)");
        
        // Draw time at bottom
        float t = data[data.Count - 1].x;
        GUI.Label(new Rect(rect.x + rect.width - 50, rect.y + rect.height + 5, 50, 20), $"t={t:F1}s");
    }
    
    private string GetSystemType(float damping)
    {
        if (damping < 1.0f - Mathf.Epsilon)
            return "Underdamped (Oscillating)";
        else if (damping > 1.0f + Mathf.Epsilon)
            return "Overdamped (No Oscillation)";
        else
            return "Critically Damped";
    }
} 
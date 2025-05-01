using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Modules.Maths;

[CustomEditor(typeof(SecondOrderVisualizer))]
public class SecondOrderVisualizerEditor : Editor
{
    private const int graphHeight = 140;
    private const int graphWidth = 300;
    private bool showSystemResponse = true;
    private bool showComparison = false;
    private EquationSolverType[] selectedSolversForComparison = new EquationSolverType[0];
    private Color[] lineColors = new Color[] 
    {
        Color.cyan,      // Main line color
        Color.red,       // Comparison line 1
        Color.green,     // Comparison line 2
        Color.yellow     // Comparison line 3
    };

    public override void OnInspectorGUI()
    {
        SecondOrderVisualizer visualizer = (SecondOrderVisualizer)target;
        
        // Check for parameter changes
        EditorGUI.BeginChangeCheck();
        
        // Draw default inspector
        DrawDefaultInspector();
        
        bool changed = EditorGUI.EndChangeCheck();
        if (changed)
        {
            // If parameters were changed, update the simulation
            visualizer.InitializeSolvers();
        }
        
        EditorGUILayout.Space(10);
        
        // Add reset button
        if (GUILayout.Button("Reset Simulation"))
        {
            visualizer.ResetSimulation();
        }
        
        EditorGUILayout.Space(10);
        
        // Main graph foldout
        showSystemResponse = EditorGUILayout.Foldout(showSystemResponse, "System Response Visualization", true, EditorStyles.foldoutHeader);
        if (showSystemResponse)
        {
            GUILayout.Label($"Current Solver: {visualizer.solverType}");
            
            // Draw main graph
            Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
            DrawGraph(rect, visualizer.GetCurrentDataPoints(), lineColors[0]);
            
            // Draw reference line
            Handles.BeginGUI();
            Handles.color = Color.green;
            Handles.DrawLine(
                new Vector3(rect.x, rect.y + rect.height / 2),
                new Vector3(rect.x + rect.width, rect.y + rect.height / 2)
            );
            Handles.EndGUI();
            
            // Display current parameters
            EditorGUILayout.LabelField($"f = {visualizer.frequency:F2}, z = {visualizer.damping:F2}, r = {visualizer.response:F2}");
        }
        
        EditorGUILayout.Space(10);
        
        // Solver comparison foldout
        showComparison = EditorGUILayout.Foldout(showComparison, "Solver Comparison", true, EditorStyles.foldoutHeader);
        if (showComparison)
        {
            // Allow selection of solvers to compare
            EditorGUILayout.LabelField("Select solvers to compare:", EditorStyles.boldLabel);
            
            List<EquationSolverType> selectedSolvers = new List<EquationSolverType>();
            
            foreach (EquationSolverType solverType in System.Enum.GetValues(typeof(EquationSolverType)))
            {
                bool isSelected = EditorGUILayout.ToggleLeft(solverType.ToString(), 
                    System.Array.IndexOf(selectedSolversForComparison, solverType) >= 0);
                
                if (isSelected && !selectedSolvers.Contains(solverType))
                {
                    selectedSolvers.Add(solverType);
                }
            }
            
            selectedSolversForComparison = selectedSolvers.ToArray();
            
            if (selectedSolversForComparison.Length > 0)
            {
                // Draw comparison graph
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Comparison Graph:", EditorStyles.boldLabel);
                
                Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
                
                // Draw baseline for comparison
                Handles.BeginGUI();
                Handles.color = Color.green;
                Handles.DrawLine(
                    new Vector3(rect.x, rect.y + rect.height / 2),
                    new Vector3(rect.x + rect.width, rect.y + rect.height / 2)
                );
                Handles.EndGUI();
                
                // Draw each selected solver's data
                for (int i = 0; i < selectedSolversForComparison.Length; i++)
                {
                    EquationSolverType type = selectedSolversForComparison[i];
                    Color lineColor = lineColors[(i % (lineColors.Length - 1)) + 1]; // Skip first color (main graph)
                    
                    DrawGraph(rect, visualizer.GetDataPoints(type), lineColor);
                    
                    // Draw legend
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ColorField(GUIContent.none, lineColor, false, false, false, GUILayout.Width(20));
                    EditorGUILayout.LabelField(type.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }

    private void DrawGraph(Rect rect, List<float> data, Color lineColor)
    {
        if (data == null || data.Count < 2) return;

        Handles.BeginGUI();
        Handles.color = lineColor;

        // Find min/max of the data for scaling
        float min = Mathf.Min(data.ToArray());
        float max = Mathf.Max(data.ToArray());
        
        // Ensure reasonable graph bounds
        float range = Mathf.Max(1.0f, max - min);
        
        // Adjust for center line to represent zero
        float centerY = rect.y + rect.height / 2;

        for (int i = 1; i < data.Count; i++)
        {
            float x0 = rect.x + (i - 1) * (rect.width / (data.Count - 1));
            float y0 = centerY - (data[i - 1] / range) * (rect.height / 2);

            float x1 = rect.x + i * (rect.width / (data.Count - 1));
            float y1 = centerY - (data[i] / range) * (rect.height / 2);

            Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
        }

        Handles.EndGUI();
    }
} 
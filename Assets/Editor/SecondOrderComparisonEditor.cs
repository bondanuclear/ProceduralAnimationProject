using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Modules.Maths;

[CustomEditor(typeof(SecondOrderComparison))]
public class SecondOrderComparisonEditor : Editor
{
    private const int graphHeight = 300;
    private const int graphWidth = 500;
    
    private bool showSettings = true;
    private bool showComparison = true;
    private bool showErrorAnalysis = false;
    
    // Color mapping for the different solvers
    private readonly Dictionary<string, Color> lineColors = new Dictionary<string, Color>
    {
        { "Theoretical", Color.white },
        { nameof(EquationSolverType.EulerStable), new Color(1f, 0.5f, 0f) },  // Orange
        { nameof(EquationSolverType.EulerStableCorrectPhysics), new Color(0f, 1f, 0f) }, // Green
        { nameof(EquationSolverType.SemiImplicitEuler), new Color(0f, 0.7f, 1f) }, // Cyan
        { nameof(EquationSolverType.VerletIntegration), new Color(1f, 0.7f, 0.8f) } // Pink
    };

    public override void OnInspectorGUI()
    {
        SecondOrderComparison comparison = (SecondOrderComparison)target;
        
        // Check for parameter changes
        EditorGUI.BeginChangeCheck();
        
        // Settings section
        showSettings = EditorGUILayout.Foldout(showSettings, "Simulation Settings", true, EditorStyles.foldoutHeader);
        if (showSettings)
        {
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
            
            // Draw parameter fields
            comparison.frequency = EditorGUILayout.Slider("Frequency (f)", comparison.frequency, 0.1f, 10f);
            comparison.damping = EditorGUILayout.Slider("Damping (z)", comparison.damping, 0f, 2f);
            comparison.response = EditorGUILayout.Slider("Response (r)", comparison.response, -5f, 5f);
            
            EditorGUILayout.Space(5);
            
            // Draw simulation settings
            EditorGUILayout.LabelField("Simulation", EditorStyles.boldLabel);
            comparison.resolution = EditorGUILayout.IntSlider("Resolution", comparison.resolution, 100, 500);
            comparison.simulationTime = EditorGUILayout.Slider("Simulation Time (s)", comparison.simulationTime, 0.5f, 5f);
            comparison.autoSimulate = EditorGUILayout.Toggle("Auto Simulate on Change", comparison.autoSimulate);
            
            EditorGUILayout.Space(5);
            
            // Draw visibility toggles
            EditorGUILayout.LabelField("Visibility", EditorStyles.boldLabel);
            comparison.showTheoreticalCurve = EditorGUILayout.Toggle("Show Theoretical", comparison.showTheoreticalCurve);
            comparison.showEulerStable = EditorGUILayout.Toggle("Show Euler Stable", comparison.showEulerStable);
            comparison.showEulerStableCorrectPhysics = EditorGUILayout.Toggle("Show Euler Stable CP", comparison.showEulerStableCorrectPhysics);
            comparison.showSemiImplicitEuler = EditorGUILayout.Toggle("Show Semi-Implicit Euler", comparison.showSemiImplicitEuler);
            comparison.showVerletIntegration = EditorGUILayout.Toggle("Show Verlet Integration", comparison.showVerletIntegration);
            
            EditorGUILayout.Space(5);
            
            // Button to manually run simulation
            if (GUILayout.Button("Run Simulation"))
            {
                comparison.RunSimulation();
            }
        }
        
        // Apply changes if parameters were modified
        bool changed = EditorGUI.EndChangeCheck();
        if (changed && comparison.autoSimulate)
        {
            comparison.RunSimulation();
        }
        
        EditorGUILayout.Space(10);
        
        // Comparison section
        showComparison = EditorGUILayout.Foldout(showComparison, "Solver Comparison", true, EditorStyles.foldoutHeader);
        if (showComparison)
        {
            string damping = GetDampingType(comparison.damping);
            string fValue = $"{comparison.frequency:F2}";
            string zValue = $"{comparison.damping:F2}";
            string rValue = $"{comparison.response:F2}";
            
            EditorGUILayout.LabelField($"System Type: {damping}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Parameters: f={fValue}, z={zValue}, r={rValue}");
            
            EditorGUILayout.Space(5);
            
            // Draw graph comparing all solvers
            Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
            DrawComparisonGraph(rect, comparison);
            
            // Draw legend
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Legend:", EditorStyles.boldLabel);
            
            // Theoretical curve
            if (comparison.showTheoreticalCurve)
            {
                DrawLegendItem("Theoretical (Exact)", lineColors["Theoretical"]);
            }
            
            // Euler Stable
            if (comparison.showEulerStable)
            {
                DrawLegendItem("Euler Stable", lineColors[nameof(EquationSolverType.EulerStable)]);
            }
            
            // Euler Stable Correct Physics
            if (comparison.showEulerStableCorrectPhysics)
            {
                DrawLegendItem("Euler Stable CP", lineColors[nameof(EquationSolverType.EulerStableCorrectPhysics)]);
            }
            
            // Semi-Implicit Euler
            if (comparison.showSemiImplicitEuler)
            {
                DrawLegendItem("Semi-Implicit Euler", lineColors[nameof(EquationSolverType.SemiImplicitEuler)]);
            }
            
            // Verlet Integration
            if (comparison.showVerletIntegration)
            {
                DrawLegendItem("Verlet Integration", lineColors[nameof(EquationSolverType.VerletIntegration)]);
            }
        }
        
        EditorGUILayout.Space(10);
        
        // Error analysis section
        showErrorAnalysis = EditorGUILayout.Foldout(showErrorAnalysis, "Error Analysis", true, EditorStyles.foldoutHeader);
        if (showErrorAnalysis)
        {
            EditorGUILayout.LabelField("Root Mean Square Error vs. Theoretical:", EditorStyles.boldLabel);
            
            // Calculate RMSE for each solver
            Dictionary<EquationSolverType, float> errors = CalculateErrors(comparison);
            
            foreach (var entry in errors)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Color box
                EditorGUILayout.ColorField(GUIContent.none, lineColors[entry.Key.ToString()], false, false, false, GUILayout.Width(20));
                
                // Solver name and error
                EditorGUILayout.LabelField($"{entry.Key}: {entry.Value:F5} RMSE");
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Lower RMSE indicates higher accuracy compared to the theoretical response.", MessageType.Info);
        }
    }

    private void DrawComparisonGraph(Rect rect, SecondOrderComparison comparison)
    {
        // Draw graph background
        EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));
        
        Handles.BeginGUI();
        
        // Draw X and Y axes
        Handles.color = new Color(0.3f, 0.3f, 0.3f);
        
        // X-axis
        float centerY = rect.y + rect.height / 2;
        Handles.DrawLine(
            new Vector3(rect.x, centerY),
            new Vector3(rect.x + rect.width, centerY)
        );
        
        // Y-axis
        Handles.DrawLine(
            new Vector3(rect.x, rect.y),
            new Vector3(rect.x, rect.y + rect.height)
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
        
        // Get all data
        List<Vector2> theoreticalData = comparison.GetTheoreticalData();
        
        // Find global min/max for scaling
        float maxY = 0f;
        
        // Check theoretical data
        if (comparison.showTheoreticalCurve && theoreticalData.Count > 0)
        {
            foreach (Vector2 point in theoreticalData)
            {
                maxY = Mathf.Max(maxY, Mathf.Abs(point.y));
            }
        }
        
        // Check solver data
        if (comparison.showEulerStable)
            maxY = UpdateMaxY(comparison.GetSolverData(EquationSolverType.EulerStable), maxY);
            
        if (comparison.showEulerStableCorrectPhysics)
            maxY = UpdateMaxY(comparison.GetSolverData(EquationSolverType.EulerStableCorrectPhysics), maxY);
            
        if (comparison.showSemiImplicitEuler)
            maxY = UpdateMaxY(comparison.GetSolverData(EquationSolverType.SemiImplicitEuler), maxY);
            
        if (comparison.showVerletIntegration)
            maxY = UpdateMaxY(comparison.GetSolverData(EquationSolverType.VerletIntegration), maxY);
        
        // Ensure reasonable scale
        maxY = Mathf.Max(maxY, 1.0f);
        
        // Calculate scale factors
        float timeScale = rect.width / comparison.simulationTime;
        float yScale = (rect.height / 2) / maxY;
        
        // Draw target line (y = response)
        Handles.color = Color.green;
        float targetY = centerY - comparison.response * yScale;
        Handles.DrawLine(
            new Vector3(rect.x, targetY),
            new Vector3(rect.x + rect.width, targetY)
        );
        
        // Draw theoretical data
        if (comparison.showTheoreticalCurve && theoreticalData.Count > 1)
        {
            DrawCurve(rect, theoreticalData, timeScale, centerY, yScale, lineColors["Theoretical"]);
        }
        
        // Draw solver data
        if (comparison.showEulerStable)
            DrawSolverCurve(comparison, rect, EquationSolverType.EulerStable, timeScale, centerY, yScale);
            
        if (comparison.showEulerStableCorrectPhysics)
            DrawSolverCurve(comparison, rect, EquationSolverType.EulerStableCorrectPhysics, timeScale, centerY, yScale);
            
        if (comparison.showSemiImplicitEuler)
            DrawSolverCurve(comparison, rect, EquationSolverType.SemiImplicitEuler, timeScale, centerY, yScale);
            
        if (comparison.showVerletIntegration)
            DrawSolverCurve(comparison, rect, EquationSolverType.VerletIntegration, timeScale, centerY, yScale);
        
        // Draw time labels
        GUI.Label(new Rect(rect.x, rect.y + rect.height + 2, 20, 20), "0");
        GUI.Label(new Rect(rect.x + rect.width - 30, rect.y + rect.height + 2, 30, 20), 
            $"{comparison.simulationTime:F1}s");
        
        Handles.EndGUI();
    }
    
    private float UpdateMaxY(List<Vector2> data, float currentMax)
    {
        if (data.Count == 0) return currentMax;
        
        foreach (Vector2 point in data)
        {
            currentMax = Mathf.Max(currentMax, Mathf.Abs(point.y));
        }
        
        return currentMax;
    }
    
    private void DrawCurve(Rect rect, List<Vector2> data, float timeScale, float centerY, float yScale, Color color)
    {
        if (data.Count < 2) return;
        
        Handles.color = color;
        
        for (int i = 1; i < data.Count; i++)
        {
            float x0 = rect.x + data[i - 1].x * timeScale;
            float y0 = centerY - data[i - 1].y * yScale;
            
            float x1 = rect.x + data[i].x * timeScale;
            float y1 = centerY - data[i].y * yScale;
            
            Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
        }
    }
    
    private void DrawSolverCurve(SecondOrderComparison comparison, Rect rect, 
        EquationSolverType solverType, float timeScale, float centerY, float yScale)
    {
        List<Vector2> data = comparison.GetSolverData(solverType);
        if (data.Count < 2) return;
        
        DrawCurve(rect, data, timeScale, centerY, yScale, lineColors[solverType.ToString()]);
    }
    
    private void DrawLegendItem(string label, Color color)
    {
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, GUILayout.Width(20));
        EditorGUILayout.LabelField(label);
        
        EditorGUILayout.EndHorizontal();
    }
    
    private Dictionary<EquationSolverType, float> CalculateErrors(SecondOrderComparison comparison)
    {
        Dictionary<EquationSolverType, float> errors = new Dictionary<EquationSolverType, float>();
        List<Vector2> theoretical = comparison.GetTheoreticalData();
        
        if (theoretical.Count == 0) return errors;
        
        // Calculate RMSE for each solver
        foreach (EquationSolverType type in System.Enum.GetValues(typeof(EquationSolverType)))
        {
            List<Vector2> solverData = comparison.GetSolverData(type);
            
            if (solverData.Count == 0 || solverData.Count != theoretical.Count) continue;
            
            float sumSquaredError = 0f;
            
            for (int i = 0; i < theoretical.Count; i++)
            {
                float error = solverData[i].y - theoretical[i].y;
                sumSquaredError += error * error;
            }
            
            float rmse = Mathf.Sqrt(sumSquaredError / theoretical.Count);
            errors[type] = rmse;
        }
        
        return errors;
    }
    
    private string GetDampingType(float damping)
    {
        if (damping < 1.0f - Mathf.Epsilon)
            return "Underdamped System (Oscillating)";
        else if (damping > 1.0f + Mathf.Epsilon)
            return "Overdamped System (No Oscillation)";
        else
            return "Critically Damped System";
    }
} 
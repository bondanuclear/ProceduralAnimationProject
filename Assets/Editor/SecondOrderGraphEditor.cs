using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(SecondOrderGraph))]
public class SecondOrderGraphEditor : Editor
{
    private const int graphHeight = 120;
    private const int graphWidth = 250;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SecondOrderGraph graph = (SecondOrderGraph)target;

        GUILayout.Space(10);
        GUILayout.Label("Second-Order System Response", EditorStyles.boldLabel);

        Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
        DrawGraph(rect, graph.GetGraphData());
    }

    private void DrawGraph(Rect rect, List<Vector2> data)
    {
        if (data.Count < 2) return;

        Handles.BeginGUI();
        Handles.color = Color.green;

        float minY = -1.2f, maxY = 1.2f; // Fixed Y range for consistent visualization
        float rangeY = maxY - minY;

        for (int i = 1; i < data.Count; i++)
        {
            float x0 = rect.x + (data[i - 1].x / data[data.Count - 1].x) * rect.width;
            float y0 = rect.yMax - ((data[i - 1].y - minY) / rangeY) * rect.height;

            float x1 = rect.x + (data[i].x / data[data.Count - 1].x) * rect.width;
            float y1 = rect.yMax - ((data[i].y - minY) / rangeY) * rect.height;

            Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
        }

        Handles.EndGUI();
    }
}

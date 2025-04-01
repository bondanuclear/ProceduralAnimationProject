using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(SecondOrderDemo))]
public class SecondOrderDemoEditor : Editor
{
    private const int graphHeight = 100;
    private const int graphWidth = 256;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw all fields normally

        SecondOrderDemo demo = (SecondOrderDemo)target;

        GUILayout.Space(10);
        GUILayout.Label("X (Transform) Visualization", EditorStyles.boldLabel);

        Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
        DrawGraph(rect, demo.GetDataPoints());
    }

    private void DrawGraph(Rect rect, List<float> data)
    {
        if (data.Count < 2) return;

        Handles.BeginGUI();
        Handles.color = Color.green;

        float min = Mathf.Min(data.ToArray());
        float max = Mathf.Max(data.ToArray());
        float range = Mathf.Max(0.0001f, max - min); // prevent division by zero

        for (int i = 1; i < data.Count; i++)
        {
            float x0 = rect.x + (i - 1) * (rect.width / (data.Count - 1));
            float y0 = rect.yMax - ((data[i - 1] - min) / range) * rect.height;

            float x1 = rect.x + i * (rect.width / (data.Count - 1));
            float y1 = rect.yMax - ((data[i] - min) / range) * rect.height;

            Handles.DrawLine(new Vector3(x0, y0), new Vector3(x1, y1));
        }

        Handles.EndGUI();
    }
}

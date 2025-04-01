using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RealTimeGraph : MonoBehaviour
{
    public SecondOrderSimulator simulator;
    public RectTransform graphContainer;
    public GameObject pointPrefab;

    private List<GameObject> points = new List<GameObject>();

    private void Update()
    {
        DrawGraph(simulator.GetGraphData());
    }

    private void DrawGraph(List<Vector2> data)
    {
        // Remove old points
        foreach (var point in points) Destroy(point);
        points.Clear();

        if (data.Count < 2) return;

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float minX = data[0].x, maxX = data[data.Count - 1].x;
        float minY = -1.2f, maxY = 1.2f; // Keep Y values normalized
        float rangeY = maxY - minY;

        for (int i = 0; i < data.Count; i++)
        {
            float xPos = (data[i].x - minX) / (maxX - minX) * graphWidth;
            float yPos = (data[i].y - minY) / rangeY * graphHeight;

            GameObject point = Instantiate(pointPrefab, graphContainer);
            point.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
            points.Add(point);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using Modules.Maths;

public class SecondOrderDemo : MonoBehaviour
{
    [Header("Second Order Parameters")]
    public float f = 1f; // Frequency
    public float z = 0.5f; // Damping
    public float r = 2f; // Response
    public Transform target;

    public enum UpdateMode { Update, FixedUpdate }
    public UpdateMode updateMode;

    private List<float> dataPoints = new List<float>();
    private float timeAccumulator = 0f;
    private const float maxTime = 2f; // seconds
    private const int resolution = 256; // number of points

    private IEquationSolver dynamics;

    private void Start()
    {
        dynamics = new SemiImplicitEuler(f, z, r, transform.position);
    }

    private void Update()
    {
        if (updateMode != UpdateMode.Update) return;

        UpdateLogic(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (updateMode != UpdateMode.FixedUpdate) return;

        UpdateLogic(Time.fixedDeltaTime);
    }

    void UpdateLogic(float dt)
    {
        if (target == null) return;

        Vector3 updatedPosition = dynamics.UpdateValues(target.position, null, dt);
        transform.position = updatedPosition;

        // Store just the x position
        timeAccumulator += dt;
        if (dataPoints.Count >= resolution)
            dataPoints.RemoveAt(0);

        dataPoints.Add(updatedPosition.x);
    }

    public List<float> GetDataPoints() => dataPoints;
}

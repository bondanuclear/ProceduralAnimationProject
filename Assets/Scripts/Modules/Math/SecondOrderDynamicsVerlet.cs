using UnityEngine;

public class SecondOrderDynamicsVerlet : IEquationSolver
{
    private Vector3 y, yd, yPrev;
    private float k1, k2, k3;
    private float deltaTime;

    public SecondOrderDynamicsVerlet(float f, float z, float r, Vector3 x0, float dt)
    {
        k1 = z / (f * Mathf.PI);
        k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
        k3 = (r * z) / (2 * Mathf.PI * f);

        y = x0;
        yPrev = x0; // Для Верле потрібне попереднє значення
        yd = Vector3.zero;
        deltaTime = dt;
    }

    public Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
    {
        if (xd == null)
        {
            xd = (x - y) / deltaTime;
        }

        // Обчислення прискорення згідно з рівнянням
        Vector3 acceleration = (x + k3 * xd.Value - y - k1 * yd) / k2;

        // Інтегрування за методом Верле:
        Vector3 newY = 2 * y - yPrev + (acceleration * deltaTime * deltaTime);

        // Оновлюємо змінні
        yPrev = y;
        y = newY;
        yd = (y - yPrev) / deltaTime;

        return y;
    }
}

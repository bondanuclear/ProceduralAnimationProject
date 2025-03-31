using UnityEngine;

public class SecondOrderDynamicsVerlet : IEquationSolver
{
    private Vector3 y, yd, yPrev;
    private float k1, k2, k3;
    Vector3 xp;
    
    public SecondOrderDynamicsVerlet(float f, float z, float r, Vector3 x0)
    {
        k1 = z / (f * Mathf.PI);
        k2 = 1 / (2 * Mathf.PI * f * (2 * Mathf.PI * f));
        k3 = r * z / (2 * Mathf.PI * f);
        xp = x0;
        y = x0;
        yPrev = x0; // Для Верле потрібне попереднє значення
        yd = Vector3.zero;
       
    }

    public Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
    {
        if (xd == null)
        {
            xd = (x - xp) / T;
            xp = x;
        }

        Vector3 a = (x + k3 * xd.Value - y - k1 * yd) / k2;

        Vector3 newY = 2 * y - yPrev + T * T * a;

        yd = (newY - yPrev) / (2 * T);

        yPrev = y;
        y = newY;

        return y;
    }


    // public Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
    // {
    //     if (xd == null)
    //     {
    //         xd = (x - y) / T;
    //     }

    //     // Обчислення прискорення згідно з рівнянням
    //     Vector3 acceleration = (x + k3 * xd.Value - y - k1 * yd) / k2;

    //     // Інтегрування за методом Верле:
    //     Vector3 newY = 2 * y - yPrev + (acceleration * T * T);

    //     // Оновлюємо змінні
    //     yPrev = y;
    //     y = newY;
    //     yd = (y - yPrev) / T;

    //     return y;
    // }
}

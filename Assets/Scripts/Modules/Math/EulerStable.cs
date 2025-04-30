using UnityEngine;
namespace Modules.Maths
{
    public class EulerStable : IEquationSolver
    {
        private Vector3 xp;
        private Vector3 y, yd;
        private float k1, k2, k3;

        public EulerStable(float f, float z, float r, Vector3 x0)
        {
            k1 = z / (f * Mathf.PI);
            k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
            k3 = (r * z) / (2 * Mathf.PI * f);

            xp = x0;
            y = x0;
            yd = Vector3.zero;
        }

        /// <summary>
        /// Stable Semi-Implicit Euler's method for solving differential equations.
        /// </summary>
        /// <param name="T">Time step</param>
        /// <param name="x">Input position</param>
        /// <param name="xd">Derivative of input position</param>
        /// <returns></returns>
        public Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity)
        {
            Debug.LogError("UpdateValues EulerStable");
            if (xd == null)
            {
                xd = (x - xp) / T;
                
                xp = x;
            }

            float k2_stable = Mathf.Max(k2, 1.1f * (T*T/4 + T*k1/2));
        
            y = y + T * yd;
            yd = yd + T * (x + k3 * xd.Value - y - k1 * yd) / k2_stable;
        
            return y;
        }    
    }
}

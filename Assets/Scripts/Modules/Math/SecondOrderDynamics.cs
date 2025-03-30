using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Modules.Math
{
    public class SecondOrderDynamics
    {
        private Vector3 xp;
        private Vector3 y, yd;
        private float k1, k2, k3;

        public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
        {
            k1 = z / (f * Mathf.PI);
            k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
            k3 = (r * z) / (2 * Mathf.PI * f);

            xp = x0;
            y = x0;
            yd = Vector3.zero;
        }
        /// <summary>
        /// Semi-Implicit Euler's method for solving differential equations.
        /// </summary>
        /// <param name="T">Time</param>
        /// <param name="x">Input position</param>
        /// <param name="xd">Derivative of input position</param>
        /// <returns></returns>
        public Vector3 UpdateValuesEuler(float T, Vector3 x, Vector3? xd = null)
        {
            if (xd == null)
            {
                xd = (x - xp) / T;
                //Debug.Log(xd + " x derivative");
                xp = x;
            }
            y = y + T * yd;
            yd = yd + T * (x + k3 * xd.Value - y - k1 * yd) / k2;
            //Debug.Log("Y derivative is " + yd);
            return y;
        }
        /// <summary>
        /// To do: implement verlet integration.
        /// </summary>
        public void UpdateValuesVerlet()
        {
            
        }
    }

}

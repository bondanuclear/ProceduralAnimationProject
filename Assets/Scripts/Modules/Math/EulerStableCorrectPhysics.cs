using System;
using UnityEngine;
namespace Modules.Maths
{
    public class EulerStableCorrectPhysics : IEquationSolver
    {
        private const float SafetyThreshold = 0.8f;
        private Vector3 xp;
        private Vector3 y, yd;
        private float k1, k2, k3;
        private float T_critical;
        public EulerStableCorrectPhysics(float f, float z, float r, Vector3 x0)
        {
            k1 = z / (f * Mathf.PI);
            k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
            k3 = (r * z) / (2 * Mathf.PI * f);
            T_critical = (float)(SafetyThreshold * (Math.Sqrt(4*k2 + k1 * k1) - k1));
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
            if (xd == null)
            {
                xd = (x - xp) / T;
                
                xp = x;
            }
            // якщо T занадто великий, ми розбиваємо його на менші кроки, які менше за поріг стабільності
            Debug.Log("T is " + T);
            int iterations = (int)Math.Ceiling(T / T_critical);
          
            Debug.Log("Iterations " + iterations);
            T = T / iterations;

            for(int i = 0; i < iterations; i++)
            {
                y = y + T * yd;
                yd = yd + T * (x + k3 * xd.Value - y - k1 * yd) / k2;
            }  
            return y;
        }    
    }
}
using UnityEngine;

public class PoleZeroMatching : IEquationSolver
{
    public Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = float.PositiveInfinity)
    {
       return Vector3.zero;
    }
}
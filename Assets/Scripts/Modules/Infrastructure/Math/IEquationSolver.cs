using UnityEngine;

public interface IEquationSolver 
{
   public Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity);
}
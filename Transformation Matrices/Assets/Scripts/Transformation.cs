
using UnityEngine;

public abstract class Transformation : MonoBehaviour
{
    public Vector3 Apply(Vector3 point)
    {
        return Matrix.MultiplyPoint(point);// automatically sets 4th coord to 1
    }
    
    public abstract Matrix4x4 Matrix { get; }
}

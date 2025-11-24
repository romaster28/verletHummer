using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ToZeroY(this Vector3 vector)
    {
        var result = new Vector3(vector.x, 0, vector.z);
        return result;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Util
{
    public static bool AreEqual<T>(T prev, T check) 
    {
        return Equals(prev, check);
    }
    
    public static float CalculateDiagonal(float width, float height)
    {
        return (Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)));
    }
    
    public static bool IsInRangeOf<T>(this int index, IList<T> list)
    {
        return index > -1 && index < list.Count;
    }

    public static Vector3 AveragePosition(Vector3[] vectors)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        foreach (var v in vectors)
        {
            x += v.x;
            y += v.y;
            z += v.z;
        }

        return new Vector3(x / vectors.Length, y / vectors.Length, z / vectors.Length);
    }
}

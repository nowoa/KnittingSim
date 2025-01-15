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
        Vector3 vector = new();
        foreach (var v in vectors)
        {
            vector += v;
        }

        return new Vector3(vector.x / vectors.Length, vector.y / vectors.Length, vector.z / vectors.Length);
    }

    public static bool IsEven(this int i)
    {
        return i % 2 == 0;
    }
}

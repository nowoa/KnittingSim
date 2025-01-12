using System.Collections;
using System.Collections.Generic;
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
}

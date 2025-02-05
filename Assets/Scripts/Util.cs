using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

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

    public static VerletNode Traverse(this VerletNode node, VerletNode.Neighbor direction)
    {
        return node.Neighbors[(int)direction];
    }

    public static float Fract(this float f)
    {
        return f - Mathf.Floor(f);
    }

    public static bool In<T1>(this T1 item, IEnumerable<T1> collection)
    {
        return collection.Any(x => Equals(x, item));
    }
    public static bool In<T1>(this T1 item, params T1[] checkItems)
    {
        return item.In(collection: checkItems);
    }
    
    public static float Remap(this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static Vector2 Remap(this Vector2 value, Vector2 fromMin, Vector2 fromMax, Vector2 toMin, Vector2 toMax) {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    public static string Format<T>(this IEnumerable<T> items)
    {
        return "{" + string.Join(", ", items) + "}";
    }

    public static void Print<T>(this IEnumerable<T> items, string title = "")
    {
        Debug.Log(title + items.Format());
    }
}

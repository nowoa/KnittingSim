using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Verlet;
using Vector3 = UnityEngine.Vector3;

public static class Calculation
{
    public static int GetIndexFromCoordinate(int xCoordinate, int yCoordinate, int width)
    {
        return yCoordinate * width + xCoordinate;
    }
    
    public static bool GetRibValue(StitchScript stitch, int xCoordinate, int knit, int purl)
    {
        return stitch.isKnit = xCoordinate % (purl + knit) > purl - 1;
    }

    public static Vector3 GetStitchPosition(List<VerletNode> corners)
    {
        var x = 0f;
        var y = 0f;
        var z = 0f;
        foreach (var n in corners)
        {
            x += n.Position.x;
            y += n.Position.y;
            z += n.Position.z;
        }

        x /= corners.Count;
        y /= corners.Count;
        z /= corners.Count;

        return new Vector3(x, y, z);
    }

    public static float CalculateDiagonal(float width, float height)
    {
        return (Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)));
    }
    
}
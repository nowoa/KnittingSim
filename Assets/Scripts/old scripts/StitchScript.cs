using UnityEngine;
using UnityEngine.Serialization;

public class StitchScript : MonoBehaviour
{
    [FormerlySerializedAs("_width")] public float width = 2;
    [FormerlySerializedAs("_height")] public float height = 2;
    public int xCoordinate;
    public  int yCoordinate;
    [FormerlySerializedAs("_isKnit")] public bool isKnit;
    public StitchScript stitchLeft;
    public StitchScript stitchBelow;
    public StitchScript stitchRight;
    public StitchScript stitchAbove;
    public bool drawNormals;

    public static Vector3 GetNormal(Vector3 leftPos, Vector3 rightPos, Vector3 bottomPos, Vector3 topPos)
    {
        Vector3 horizontal = leftPos - rightPos;
        Vector3 vertical = topPos - bottomPos;
        return Vector3.Cross(horizontal, vertical).normalized;
    }
    
    public Vector3 GetNormal()
    {
        var leftPos1 = stitchLeft == null ? transform.position : stitchLeft.transform.position;
        var rightPos1 = stitchRight == null ? transform.position : stitchRight.transform.position;
        var topPos1 = stitchAbove == null ? transform.position : stitchAbove.transform.position;
        var bottomPos1 = stitchBelow == null ? transform.position : stitchBelow.transform.position;
        return GetNormal(leftPos1, rightPos1, bottomPos1,
                topPos1);
    }
    
    
    
    public static Vector3 GetAverage(Vector3 stitch, Vector3 otherStitch)
    {
        return (stitch + otherStitch) / 2;
    }
}


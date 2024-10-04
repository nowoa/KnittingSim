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
    /*private void OnDrawGizmos()
    {
        Vector3 leftPos;
        Vector3 rightPos;
        Color leftColor = new Color(1,0,0,0.5f);
        Color rightColor = new Color(0, 0, 1, 0.5f);
        if (stitchLeft != null)
        {
            var position = transform.position;
            leftPos=GetAverage(position, stitchLeft.transform.position);
            Gizmos.color = leftColor;
            Gizmos.DrawSphere(leftPos,0.05f);
            
            Gizmos.DrawLine(leftPos,position);
        }

        if (stitchRight != null)
        {
            var position = transform.position;
            rightPos = GetAverage(position, stitchRight.transform.position);
            Gizmos.color = rightColor;
            Gizmos.DrawSphere(rightPos,0.05f);
            
            Gizmos.DrawLine(position,rightPos);
        }

        //stitch center
        Gizmos.color = Color.white; 
        Gizmos.DrawSphere(transform.position,0.1f);
        
        //normal vectors
        if(drawNormals)
        {
            
            Gizmos.color = Color.yellow;
            var position = transform.position;
            Gizmos.DrawLine(position, position+GetNormal());
        }
    }*/
    
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


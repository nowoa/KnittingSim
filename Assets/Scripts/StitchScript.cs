using UnityEngine;
using UnityEngine.Serialization;

public class StitchScript : MonoBehaviour
{
    [FormerlySerializedAs("_width")] public float width = 2;
    [FormerlySerializedAs("_height")] public float height = 2;


    public int xCoordinate;
    public  int yCoordinate;
    
    public bool _isKnit;
    public StitchScript stitchLeft;
    public StitchScript stitchBelow;
    public StitchScript stitchRight;
    public StitchScript stitchAbove;
    public GridMaker gridMaker;



    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    
    public void Init(GridMaker gridMaker)
    {
        this.gridMaker = gridMaker;



       

       
    }
    public float GetDepth()
    {
        return _isKnit ? 1 : -1;
    }

    private void OnDrawGizmos()
    {
        Vector3 leftPos;
        Vector3 rightPos;
        Color leftColor = new Color(1,0,0,0.5f);
        Color rightColor = new Color(0, 0, 1, 0.5f);
        if (stitchLeft != null)
        {
            leftPos=GetAverage(this.GetDisplacement(), stitchLeft.GetDisplacement());
            Gizmos.color = leftColor;
            Gizmos.DrawSphere(leftPos,0.05f);
            
            Gizmos.DrawLine(leftPos,GetDisplacement());
        }

        if (stitchRight != null)
        {
            rightPos = GetAverage(this.GetDisplacement(), stitchRight.GetDisplacement());
            Gizmos.color = rightColor;
            Gizmos.DrawSphere(rightPos,0.05f);
            
            Gizmos.DrawLine(GetDisplacement(),rightPos);
        }
        ;
        //stitch center
        Gizmos.color = Color.white; 
        Gizmos.DrawSphere(GetDisplacement(),0.1f);
        
        //normal vectors
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(GetDisplacement(), GetDisplacement()+GetNormal());
    }
    
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
    
    public Vector3 GetDisplacement()
    {
        return transform.position + GetNormal() * GetDepth() * gridMaker.displacementFactor;
    }
    
    public static Vector3 GetAverage(Vector3 stitch, Vector3 otherStitch)
    {
        return (stitch + otherStitch) / 2;
    }
}


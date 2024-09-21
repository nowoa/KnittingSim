using UnityEngine;
using UnityEngine.Serialization;

public class StitchScript : MonoBehaviour
{
    [FormerlySerializedAs("_width")] public float width = 2;
    [FormerlySerializedAs("_height")] public float height = 2;
    [FormerlySerializedAs("_leftPos")] public Vector3 leftPos;

    [FormerlySerializedAs("_rightPos")] public Vector3 rightPos;
    public Vector3 centerPos;

    public int xCoordinate;
    public  int yCoordinate;
    
    public bool _isKnit;
    public StitchScript stitchLeft;
    public StitchScript stitchBelow;



    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    
    public void Init()
    {
        leftPos.x = transform.position.x - 0.5f * width;
        leftPos.y = transform.position.y;

        rightPos.x = transform.position.x + 0.5f * width;
        rightPos.y = transform.position.y;

        centerPos = transform.position;  
    }
    public float GetDepth()
    {
        return _isKnit ? 1 : 0;
    }

    private void OnDrawGizmos()
    {
        Color leftColor = new Color(1,0,0,0.5f);
        Color rightColor = new Color(0, 0, 1, 0.5f);
        //stitch center
        Gizmos.color = Color.white; 
        Gizmos.DrawSphere(centerPos,0.1f);
     
        //stitch leftpos
        Gizmos.color = leftColor;
        Gizmos.DrawSphere(leftPos,0.05f);
        
        Gizmos.color = rightColor;
        Gizmos.DrawSphere(rightPos,0.05f);
        
        Gizmos.DrawLine(leftPos,centerPos);
        Gizmos.DrawLine(centerPos,rightPos);
    }
    
    
}

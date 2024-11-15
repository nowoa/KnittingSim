using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Verlet;
using static Verlet.VerletNode;
using Vector3 = UnityEngine.Vector3;

public class StitchInfo
{
    public VerletNode TopLeft { get; }
    public VerletNode TopRight { get; }
    public VerletNode BottomLeft { get; }
    public VerletNode BottomRight { get; }

    public StitchInfo StitchLeft { get; private set; }
    public StitchInfo StitchRight { get; private set; }
    public StitchInfo StitchAbove { get; private set; }
    public StitchInfo StitchBelow { get; private set; }

    public Vector3 Position { get; private set; }

    private float _height;
    private float _width;
    private bool _isInactive = false;
    public bool isInactive => _isInactive;
    public float height => _height;
    public float width => _width;
    private static StitchInfo _firstDecrease;
    private static bool _firstDone;
    private StitchType _stitchType = 0;
    public StitchType stitchType => _stitchType;
    private bool knit = true;

    public enum StitchType
    {
        normal,
        DecreaseFirst,
        DecreaseMiddle,
        DecreaseLast,
        BindOff,
        CastOn,
    }

    public void SetGridPosition(StitchInfo left, StitchInfo above, StitchInfo right, StitchInfo below)
    {
        if (left != null)
        {
            StitchLeft = left;
        }
        if (right != null)
        {
            StitchRight = right;
        }
        if (above != null)
        {
            StitchAbove = above;
        }
        if (below != null)
        {
            StitchBelow = below;
        }
    }
    
    public List<VerletNode> corners { get; }

    // Constructor to initialize the position
    public StitchInfo(VerletNode bl, VerletNode tl, VerletNode tr, VerletNode br)
    {
        TopLeft = tl;
        TopRight = tr;
        BottomLeft = bl;
        BottomRight = br;
        corners = new()
        {
            bl,
            tl,
            tr,
            br
        };
    }

    public void SetPosition(Vector3 myPos)
    {
        Position = myPos;
    }

    public void SetSize(Vector2 mySize)
    {
        _height = mySize.y;
        _width = mySize.x;
    }

    public void UpdateCorners(VerletNode myNode, int myCornerIndex)
    {
        corners[myCornerIndex] = myNode;
    }

    public void RemoveStitch()
    {
        if (isInactive)
        {
            return;
        }

        corners[1].RemoveShearEdge(false); //top left
        corners[0].RemoveShearEdge(true); //bottom left

        if (corners[3].ShearEdgeUp == null) // bottom right
        {
            corners[3].RemoveStructuralEdge(true);
            corners[3].RemoveBendEdge(true);
            corners[3].NodeBelow?.RemoveBendEdge(true);
        }

        var verletNode = corners[0].NodeLeft; //bottom left
        if (verletNode == null || verletNode.ShearEdgeUp == null)
        {
            corners[0].RemoveStructuralEdge(true); //bottom left
            corners[0].RemoveBendEdge(true);
            corners[0].NodeBelow?.RemoveBendEdge(true);
        }

        if (corners[1].ShearEdgeUp == null) //top left
        {
            corners[1].RemoveStructuralEdge(false);
            corners[1].RemoveBendEdge(false);
            var nodeLeft = corners[1].NodeLeft;
            if (nodeLeft != null)
            {
                nodeLeft.RemoveBendEdge(false);
            }
        }

        
        if (corners[0].NodeBelow == null || corners[0].NodeBelow?.ShearEdgeUp == null) // Safely check ShearEdgeUp if NodeBelow is not null
        {
            corners[0].RemoveStructuralEdge(false);
            corners[0].RemoveBendEdge(false);

            var nodeLeft = corners[0].NodeLeft;
            if (nodeLeft != null)
            {
                nodeLeft.RemoveBendEdge(false);
            }
        }

        UpdateCorners(null, 0);
        UpdateCorners(null, 1);
        UpdateCorners(null, 2);
        UpdateCorners(null, 3);
        _isInactive = true;
    }

    public static void Decrease(List<StitchInfo> myStitches, bool myDirection)
    {
        _firstDecrease = myStitches.First();
        _firstDone = false;
        var targetStitch = myStitches.Last();
        if (!myDirection) //TO DO: if going left, decrease logic needs to be inverted
        {
            return;
        }
        
        for (int i = 0; i < myStitches.Count-1;i++)
        {
            var stitch = myStitches[i];
            RemoveColumn(stitch);
            ConnectDecreasedStitches(myStitches[i],targetStitch);
            if (!_firstDone)
            {
                _firstDone = true;
            }

            if (myStitches.Count > 2 && i<myStitches.Count-2 && myStitches[i].corners[1].BendEdgeHorizontal==null)
            {
                Debug.Log("bend edge connected");
                VerletEdge.ConnectNodes(myStitches[i].corners[1],myStitches[i+2].corners[1], myStitches[i].width*2);
                myStitches[i].corners[1].SetBendEdge(false);
            }
        }

        
        
        FabricManager.InvokeUpdateSimulation();
    }
    private static void RemoveColumn(StitchInfo myStitch, bool remove = false)
         {
             myStitch.corners[3].RemoveAllEdges();
             if (myStitch.StitchBelow != null)
             {
                 RemoveColumn(myStitch.StitchBelow, true); 
             }
             FabricManager.AllNodes.Remove(myStitch.corners[3]);
             if (remove)
             {
                 FabricManager.AllStitches.Remove(myStitch.corners[3].Parent);
             }
     
         }
    private static void ConnectDecreasedStitches(StitchInfo stitchToConnect, StitchInfo targetStitch)
    {
        var left = _firstDecrease.corners[0];
        var right = targetStitch.corners[3];
        if (!_firstDone)
        {
            _firstDecrease._stitchType = StitchType.DecreaseFirst;
            targetStitch._stitchType = StitchType.DecreaseLast;
            VerletEdge.ConnectNodes(left, right, targetStitch.width);
            left.SetStructuralEdge( false);
            
            _firstDecrease.UpdateCorners(right,3);
            targetStitch.UpdateCorners(left,0);
            
            VerletEdge.ConnectNodes(targetStitch.corners[0],targetStitch.corners[1],targetStitch.height);
            targetStitch.corners[0].SetStructuralEdge(true);
            
            VerletEdge.ConnectNodes(_firstDecrease.corners[0],_firstDecrease.corners[2],Calculation.CalculateDiagonal(_firstDecrease.width,_firstDecrease.height));
            _firstDecrease.corners[0].SetShearEdge(true);
            
            VerletEdge.ConnectNodes(_firstDecrease.corners[1],_firstDecrease.corners[3],Calculation.CalculateDiagonal(_firstDecrease.width, _firstDecrease.height));
            _firstDecrease.corners[1].SetShearEdge(false);
            
            VerletEdge.ConnectNodes(targetStitch.corners[0],targetStitch.corners[2],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            targetStitch.corners[0].SetShearEdge(true);
            
            VerletEdge.ConnectNodes(targetStitch.corners[1],targetStitch.corners[3],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            targetStitch.corners[1].SetShearEdge(false);
            
            
            ConnectColumns(_firstDecrease.StitchBelow,targetStitch.StitchBelow);
        }
        else
        {
            stitchToConnect._stitchType = StitchType.DecreaseMiddle;
            VerletEdge.ConnectNodes(stitchToConnect.corners[1],_firstDecrease.corners[0],stitchToConnect.height);
            VerletEdge.ConnectNodes(stitchToConnect.corners[2],targetStitch.corners[3],stitchToConnect.height);
            stitchToConnect.UpdateCorners(_firstDecrease.corners[0],0);
            stitchToConnect.UpdateCorners(targetStitch.corners[3],3);
            VerletEdge.ConnectNodes(stitchToConnect.corners[1],stitchToConnect.corners[3],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            stitchToConnect.corners[1].SetShearEdge(false);
            VerletEdge.ConnectNodes(stitchToConnect.corners[0],stitchToConnect.corners[2],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            
        }
        
        
    }
    private static void ConnectColumns(StitchInfo left, StitchInfo right)
    {
        var stitchValues = right;
        
        VerletEdge.ConnectNodes(left.corners[0],right.corners[3],stitchValues.width);
        left.corners[0].SetStructuralEdge(false);
        
        VerletEdge.ConnectNodes(left.StitchAbove.corners[0],right.corners[3],Calculation.CalculateDiagonal(stitchValues.width,stitchValues.height));
        left.StitchAbove.corners[0].SetShearEdge(false);
        
        VerletEdge.ConnectNodes(left.corners[0],right.StitchAbove.corners[3],Calculation.CalculateDiagonal(stitchValues.width,stitchValues.height));
        left.corners[0].SetShearEdge(true);

        /*if (left.NodeLeft != null && left.NodeLeft.BendEdgeHorizontal==null)
        {
            VerletEdge.ConnectNodes(left.NodeLeft, right, parent.width * 2);
            left.NodeLeft.SetBendEdge(false);
        }

        if (right.NodeRight != null && left.BendEdgeHorizontal==null)
        {
            VerletEdge.ConnectNodes(left, right.NodeRight, parent.width * 2);
            left.SetBendEdge(false);
        }*/
        
        // TO DO: figure out why the bend edges arent being removed if another decrease is made to the right
        
        left.UpdateCorners(right.StitchAbove.corners[3], 2);
        left.UpdateCorners(right.corners[3],3);

        if (left.StitchBelow != null && right.StitchBelow != null)
        {
            ConnectColumns(left.StitchBelow,right.StitchBelow);
        }
    }

   
    
}

public class PanelInfo
{
    private List<VerletNode> _nodes;
    public List<VerletNode> Nodes => _nodes;
    private int _width;
    public int Width => _width;
    private int _height;
    public int Height => _height;
    private bool _isCircular;
    public bool IsCircular => _isCircular;
    
    public PanelInfo(List<VerletNode> nodes, int width, int height, bool isCircular)
    {
        _nodes = nodes;
        _width = width;
        _height = height;
        _isCircular = isCircular;
    }
    

    public VerletNode GetNodeAt(int x, int y)
    {
        return _nodes[Calculation.GetIndexFromCoordinate(x, y, _width)];
    }
}


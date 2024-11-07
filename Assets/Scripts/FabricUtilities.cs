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
    public VerletNode topLeft { get; }
    public VerletNode topRight { get; }
    public VerletNode bottomLeft { get; }
    public VerletNode bottomRight { get; }
    private Vector3 _position;
    public Vector3 position => _position;
    private float _height;
    private float _width;
    private bool _isInactive = false;
    public bool isInactive => _isInactive;
    public float height => _height;
    public float width => _width;
    public List<VerletNode> corners { get; }

    // Constructor to initialize the position
    public StitchInfo(VerletNode bl, VerletNode tl, VerletNode tr, VerletNode br)
    {
        topLeft = tl;
        topRight = tr;
        bottomLeft = bl;
        bottomRight = br;
        corners = new()
        {
            bl,
            tl,
            tr,
            br
        };

        //will hold more information such as stitch type, inc/dec etc
    }

    public void SetPosition(Vector3 myPos)
    {
        _position = myPos;
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
            corners[3].NodeBelow.RemoveBendEdge(true);

            corners[3].SetNodeAbove(null);
            corners[2].SetNodeBelow(null);
        }

        var verletNode = corners[0].NodeLeft; //bottom left
        if (verletNode == null || verletNode.ShearEdgeUp == null)
        {
            corners[0].RemoveStructuralEdge(true); //bottom left
            corners[0].RemoveBendEdge(true);
            corners[0].NodeBelow.RemoveBendEdge(true);

            corners[0].SetNodeAbove(null);
            corners[1].SetNodeBelow(null);
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

            corners[1].SetNodeRight(null);
            corners[2].SetNodeLeft(null);
        }

        if (corners[0].NodeBelow.ShearEdgeUp==null || corners[0].NodeBelow==null) //bottom left
        {
            corners[0].RemoveStructuralEdge(false);
            corners[0].RemoveBendEdge(false);

            var nodeLeft = corners[0].NodeLeft;
            if (nodeLeft != null)
            {
                nodeLeft.RemoveBendEdge(false);
            }

            corners[0].SetNodeRight(null);
            corners[3].SetNodeLeft(null);
        }

        UpdateCorners(null, 0);
        UpdateCorners(null, 1);
        UpdateCorners(null, 2);
        UpdateCorners(null, 3);
        _isInactive = true;
    }

    public void OverlapStitches(VerletNode startPos, StitchInfo myStitchInfo)
    {
        //overlap two stitches above (only first iteration)
        if (startPos.NodeLeft != null)
        {
            var overlapStitchRight = myStitchInfo.corners[1].Parent;
            var overlapStitchLeft = overlapStitchRight.corners[0].NodeLeft.Parent;
            overlapStitchRight.corners[0].RemoveAllEdges();
            var overlapLeft = overlapStitchRight.corners[0].NodeLeft;
            var overlapRight = overlapStitchRight.corners[3];
            VerletEdge.ConnectNodes(overlapLeft,overlapRight,overlapStitchRight.width);
            overlapStitchRight.UpdateCorners(overlapLeft,0);
            overlapLeft.Parent.UpdateCorners(overlapRight,3);
            VerletEdge.ConnectNodes(overlapStitchLeft.corners[2],overlapStitchLeft.corners[3],overlapStitchLeft.height);
            VerletEdge.ConnectNodes(overlapStitchRight.corners[0],overlapStitchRight.corners[1],overlapStitchRight.height);
        }
        
        DecreaseColumn(startPos,myStitchInfo);

    }
    public void DecreaseColumn(VerletNode startPos, StitchInfo myStitchInfo) //merge column to the right
    {
        if (startPos.NodeLeft == null && startPos.NodeRight.NodeRight == null)
        {
            return;
        }
        if (startPos.NodeLeft == null) //woah this is like binding off lol
        {
            startPos.RemoveAllEdges();
            startPos.NodeRight.SetNodeLeft(null);
            if (startPos.NodeBelow !=null)
            {
                DecreaseColumn(startPos.NodeBelow, startPos.NodeBelow.Parent);
            }
            FabricManager.AllNodes.Remove(startPos);
            FabricManager.AllStitches.Remove(myStitchInfo);
            return;
        }
        
        
        
        
        var nodeLeft = startPos.NodeLeft;
        
        if (nodeLeft == null || startPos.NodeRight == null || startPos.NodeRight.NodeAbove == null || 
            nodeLeft.NodeBelow == null || startPos.Parent == null || nodeLeft.Parent == null)
        {
            Debug.Log("one of the needed nodes is null!");
            return;
        }

        
        startPos.RemoveAllEdges();
        
        VerletEdge.ConnectNodes(nodeLeft, startPos.NodeRight, myStitchInfo.width);
        nodeLeft.SetStructuralEdge(nodeLeft.Connection.Last(),false);
        
        VerletEdge.ConnectNodes(nodeLeft, startPos.NodeRight.NodeAbove,
            Calculation.CalculateDiagonal(startPos.Parent.width, startPos.Parent.height));
       nodeLeft.SetShearEdge(nodeLeft.Connection.Last(), true);
        
        VerletEdge.ConnectNodes(nodeLeft.NodeAbove, startPos.NodeRight,
            Calculation.CalculateDiagonal(startPos.Parent.width, startPos.Parent.height));
        nodeLeft.NodeAbove.SetShearEdge(nodeLeft.NodeAbove.Connection.Last(),false);
        
        nodeLeft.Parent.UpdateCorners(startPos.NodeRight, 3);
        nodeLeft.Parent.UpdateCorners(startPos.NodeRight.NodeAbove,2);
        nodeLeft.SetNodeRight(startPos.NodeRight);
        startPos.NodeRight.SetNodeLeft(nodeLeft);
        
        if (startPos.NodeBelow !=null)
        {
            /*await Task.Delay(10);*/
            DecreaseColumn(startPos.NodeBelow, startPos.NodeBelow.Parent);
            FabricManager.AllNodes.Remove(startPos);
            FabricManager.AllStitches.Remove(myStitchInfo);
        } 
        FabricManager.AllNodes.Remove(startPos);
        FabricManager.AllStitches.Remove(myStitchInfo);
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


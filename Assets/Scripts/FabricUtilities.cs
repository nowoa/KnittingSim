using System.Collections.Generic;
using System.Linq;
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
    public float height => _height;
    public float width => _width;
    public List<VerletNode> corners { get; }

    // Constructor to initialize the position
    public StitchInfo(VerletNode tl, VerletNode tr, VerletNode bl, VerletNode br)
    {
        topLeft = tl;
        topRight = tr;
        bottomLeft = bl;
        bottomRight = br;
        corners = new()
        {
            tl,
            tr,
            bl,
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

    public void RemoveStitch()
    {
        corners[0].RemoveShearEdge(false);
        corners[2].RemoveShearEdge(true);
        
        if (corners[3].ShearEdgeUp == null)
        {
            corners[3].RemoveStructuralEdge(true);
            corners[3].RemoveBendEdge(true);
            foreach (var n in corners[3].NodesBelow)
            {
                n.RemoveBendEdge(true);
            }
        }

        var verletNode = corners[2].NodeLeft;
        if (verletNode == null || verletNode.ShearEdgeUp == null)
        {
            corners[2].RemoveStructuralEdge(true);
            corners[2].RemoveBendEdge(true);
            foreach (var n in corners[2].NodesBelow)
            {
                n.RemoveBendEdge(true);
            }
        }

        if (corners[0].ShearEdgeUp == null)
        {
            corners[0].RemoveStructuralEdge(false);
            corners[0].RemoveBendEdge(false);
            var nodeLeft = corners[0].NodeLeft;
            if (nodeLeft != null)
            {
                nodeLeft.RemoveBendEdge(false);
            }
        }

        if (corners[2].NodesBelow.Any(n => n.ShearEdgeUp == null) || corners[2].NodesBelow.Count == 0)
        {
            corners[2].RemoveStructuralEdge(false);
            corners[2].RemoveBendEdge(false);
    
            var nodeLeft = corners[2].NodeLeft;
            if (nodeLeft != null)
            {
                nodeLeft.RemoveBendEdge(false);
            }
        }
    }

    public void DecreaseColumn(VerletNode startPos, StitchInfo myStitchInfo) //merge column to the right
    {
        if (startPos.NodesBelow.Count > 0)
        {
            startPos.NodesBelow.Last().RemoveAllEdges();
            VerletEdge.ConnectNodes(startPos.NodesBelow.Last().NodeLeft, startPos.NodeRight,
                myStitchInfo.width); // doesn't take into account gauge changes
            FabricManager.AllNodes.Remove(startPos.NodesBelow.Last());
            DecreaseColumn(startPos.NodesBelow.Last(), myStitchInfo);
        }
        else return;
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


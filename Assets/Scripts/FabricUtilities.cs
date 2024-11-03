using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Verlet;
using Vector3 = UnityEngine.Vector3;

public class StitchInfo
{
    public int XCoordinate { get; }
    public int YCoordinate { get; }
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
        corners[2].RemoveShearEdge();
        if (corners[3].ShearEdgeUp == null)
        {
            corners[3].RemoveVerticalEdge();
            //TO DO: remove edges based on other removed stitches
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


using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class StitchInfo
{
    public int XCoordinate { get; }
    public int YCoordinate { get; }

    // Constructor to initialize the position
    public StitchInfo( int x, int y)
    {
        XCoordinate = x;
        YCoordinate = y;
        //will hold more information such as stitch type, inc/dec etc
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


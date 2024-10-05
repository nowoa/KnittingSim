using System.Collections.Generic;
using UnityEngine;
using Verlet;
public struct StitchInfo
{
    public Vector3 Position { get; }
    public int XCoordinate { get; }
    public int YCoordinate { get; }

    // Constructor to initialize the position
    public StitchInfo(Vector3 position, int x, int y)
    {
        Position = position;
        XCoordinate = x;
        YCoordinate = y;
    }
}

public class PanelInfo
{
    private List<StitchScript> _stitches;
    public List<StitchScript> Stitches => _stitches;
    private List<VerletNode> _nodes;
    public List<VerletNode> Nodes => _nodes;
    private int _width;
    public int Width => _width;
    private int _height;
    public int Height => _height;
    private bool _isCircular;
    public bool IsCircular => _isCircular;
    private Transform _parentObject;
    public Transform ParentObject => _parentObject;
    private int _heldStitchIndex;
    public int HeldStitchIndex => _heldStitchIndex;

    public PanelInfo(List<StitchScript> stitches,List<VerletNode> nodes, int width, int height, bool isCircular, Transform parentObject, int heldStitchIndex)
    {
        _stitches = stitches;
        _nodes = nodes;
        _width = width;
        _height = height;
        _isCircular = isCircular;
        _parentObject = parentObject;
        _heldStitchIndex = heldStitchIndex;
    }

    public StitchScript GetStitchAt(int x, int y)
    {
        return _stitches[Calculation.GetIndexFromCoordinate(x, y, _width)];
    }

    public VerletNode GetNodeAt(int x, int y)
    {
        return _nodes[Calculation.GetIndexFromCoordinate(x, y, _width)];
    }
}


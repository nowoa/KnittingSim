using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;
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
    private float _elasticityFactor;
    private bool _isActive = true;
    private FabricMesh _parentMesh;

    public FabricMesh ParentMesh => _parentMesh;
    public bool IsActive => _isActive;
    public float height => _height;
    public float width => _width;
    
    private StitchType _stitchType = 0;
    public StitchType stitchType => _stitchType;
    public bool Knit = true;

    public enum StitchType
    {
        normal,
        DecreaseFirst,
        DecreaseMiddle,
        DecreaseLast,
        BindOff,
        CastOn,
    }

    public void UpdateNeighborStitch(StitchInfo stitch, string direction)
    {
        switch (direction)
        {
            case "left":
                StitchLeft = stitch;
                break;
            case "above":
                StitchAbove = stitch;
                break;
            case "right":
                StitchRight = stitch;
                break;
            case "below":
                StitchBelow = stitch;
                break;
        }
    }
    
    public List<VerletNode> Corners { get; }

    // Constructor to initialize the position
    public StitchInfo(VerletNode bl, VerletNode tl, VerletNode tr, VerletNode br)
    {
        TopLeft = tl;
        TopRight = tr;
        BottomLeft = bl;
        BottomRight = br;
        Corners = new()
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
        foreach (var c in Corners)
        {
            c.SetMarbleRadius(mySize);
        }
    }

    public void SetStitchType(StitchType type)
    {
        _stitchType = type;
    }

    public void SetInactive()
    {
        _isActive = false;
    }

    public void SetElasticityFactor(float value)
    {
        _elasticityFactor = value;
        UpdateEdgeLength();
    }
    
    public void SetParentMesh(FabricMesh parentMesh)
    {
        _parentMesh = parentMesh;
    }

    public void UpdateCorners(VerletNode myNode, int myCornerIndex)
    {
        Corners[myCornerIndex] = myNode;
    }

    private void UpdateEdgeLength()
    {
        VerletEdge.ConnectNodes(Corners[0], Corners[3], width * _elasticityFactor, VerletEdge.EdgeType.Structural);
        Corners[0].SetStructuralEdge(false);
        VerletEdge.ConnectNodes(Corners[1], Corners[2], width * _elasticityFactor,VerletEdge.EdgeType.Structural);
        Corners[1].SetStructuralEdge(false);
        VerletEdge.ConnectNodes(Corners[0],Corners[2], Calculation.CalculateDiagonal(width * _elasticityFactor, height), VerletEdge.EdgeType.Shear);
        Corners[0].SetShearEdge(true);
        VerletEdge.ConnectNodes(Corners[1],Corners[3], Calculation.CalculateDiagonal(width* _elasticityFactor, height), VerletEdge.EdgeType.Shear);
        Corners[1].SetShearEdge(false);
        if (StitchRight != null)
        {
            VerletEdge.ConnectNodes(Corners[0], StitchRight.Corners[3], width*_elasticityFactor*2, VerletEdge.EdgeType.Bend);
            Corners[0].SetBendEdge(false);
            VerletEdge.ConnectNodes(Corners[1], StitchRight.Corners[2], width*_elasticityFactor*2, VerletEdge.EdgeType.Bend);
            Corners[1].SetBendEdge(false);
        }
    }


    public void RemoveStitch()
    {
        if (stitchType == StitchType.DecreaseFirst || stitchType == StitchType.DecreaseMiddle ||
            stitchType == StitchType.DecreaseLast)
        {
            RemoveDecrease(this);
            return;
        }
        if (!IsActive)
        {
            return;
        }
        
        Corners[0].RemoveShearEdge(true);
        Corners[1].RemoveShearEdge(false);
        Corners[0].RemoveBendEdge(true);
        Corners[0].RemoveBendEdge(false);
        Corners[1].RemoveBendEdge(false);
        Corners[3].RemoveBendEdge(true);

        if (StitchLeft == null)
        {
            Corners[0].RemoveStructuralEdge(true);
            if (StitchBelow != null)
            {
                StitchBelow.Corners[0].RemoveBendEdge(true);
            }
        }

        if (StitchAbove==null)
        {
            Corners[1].RemoveStructuralEdge(false);
            if (StitchLeft!=null)
            {
                StitchLeft.Corners[1].RemoveBendEdge(false);
            }
        }

        if (StitchRight == null)
        {
            Corners[3].RemoveStructuralEdge(true);
            if (StitchBelow!=null)
            {
                StitchBelow.Corners[3].RemoveBendEdge(true);
            }
        }

        if (StitchBelow == null)
        {
            Corners[0].RemoveStructuralEdge(false);
            if (StitchLeft != null)
            {
                StitchLeft.Corners[0].RemoveBendEdge(false);
            }
        }
        foreach (var c in Corners)
        {
            if (c.Connection.Count == 0)
            {
                FabricManager.AllNodes.Remove(c);
            }
        }
        UpdateCorners(null, 0);
        UpdateCorners(null, 1);
        UpdateCorners(null, 2);
        UpdateCorners(null, 3);
        
        if (StitchAbove != null)
        {
            StitchAbove.StitchBelow = null;
        }

        if (StitchRight != null)
        {
            StitchRight.StitchLeft = null;
        }
        
        if (StitchBelow != null)
        {
            StitchBelow.StitchAbove = null;
        }

        if (StitchLeft != null)
        {
            StitchLeft.StitchRight = null;
        }

        FabricManager.AllStitches.Remove(this);
    }

    private void RemoveDecrease(StitchInfo hoveredStitch)
    {
        var stitches = Decrease.GetDecreaseStitches(hoveredStitch);
        var lastStitch = stitches.Last();
        var firstStitch = stitches.First();
        for (int i = 1; i < stitches.Count; i++)
        {
            var edge = lastStitch.Corners[0].FindEdgeByNode(stitches[i].Corners[1]);
            if (edge != null)
            {
                VerletNode.RemoveEdge(edge);
            }
        }

        for (int i = 0; i < stitches.Count - 1; i++)
        {
            var edge = lastStitch.Corners[3].FindEdgeByNode(stitches[i].Corners[2]);
            if (edge != null)
            {
                VerletNode.RemoveEdge(edge);
            }
        }

        var shearDown = lastStitch.Corners[3].FindEdgeByNode(firstStitch.Corners[1]);
        if (shearDown != null)
        {
            VerletNode.RemoveEdge(shearDown);
        }
        VerletNode.RemoveEdge(lastStitch.Corners[0].ShearEdgeUp);
        
        

        if (firstStitch.StitchLeft != null)
        {
            firstStitch.StitchLeft.StitchRight = null;
        }
        else
        {
            var edge = lastStitch.Corners[0].FindEdgeByNode(firstStitch.Corners[1]);
            if (edge != null)
            {
                VerletNode.RemoveEdge(edge);
            }
            firstStitch.Corners[0].RemoveBendEdge(true);

            if (firstStitch.StitchBelow != null)
            {
                firstStitch.StitchBelow.Corners[0].RemoveBendEdge(true);
            }
        }
        if (lastStitch.StitchRight != null)
        {
            lastStitch.StitchRight.StitchLeft = null;
        }
        else
        {
            lastStitch.Corners[3].RemoveStructuralEdge(true);
            lastStitch.Corners[3].RemoveBendEdge(true);
            if (firstStitch.StitchBelow != null)
            {
                firstStitch.StitchBelow.Corners[3].RemoveBendEdge(true);
            }
        }

        if (firstStitch.StitchBelow != null)
        {
            firstStitch.StitchBelow.StitchAbove = null;
        }
        else
        {
            lastStitch.Corners[0].RemoveStructuralEdge(false);
        }

        foreach (var s in stitches)
        {
            s._isActive = false;
            if (s.StitchAbove != null)
            {
                s.StitchAbove.StitchBelow = null;
            }
            else
            {
                s.Corners[1].RemoveStructuralEdge(false);
                s.Corners[1].RemoveBendEdge(false);
                if (s.StitchLeft != null)
                {
                    s.StitchLeft.Corners[1].RemoveBendEdge(false);
                }
            }
        }
    }

    public int GetNeighborElasticity()
    {
        var elasticity = 0;
        if (StitchLeft?.Knit != Knit)
        {
            elasticity++;
        }

        if (StitchRight?.Knit != 
            Knit)
        {
            elasticity++;
        }

        return elasticity;
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
using System.Linq;
using UnityEngine;
using Verlet;

public class Stitch
{
    private VerletNode[] _corners = new VerletNode[4];
    public VerletNode[] Corners => _corners;
    public Vector3 Position { get; private set; }
    public Panel ParentPanel { get; private set; }
    public Vector3 Normal { get; private set; }
    public StitchType stitchType { get; private set; }
    public bool Knit { get; private set; } = true;
    public int id;
    public Stitch[] Neighbors { get; private set; } = new Stitch[4];
    public float ElasticityFactor { get; private set; }
    public Vector2 Dimensions { get; private set; }

    public VerletNode GetCorner(NodeCorner cornerType) => Corners[(int)cornerType];

    public Color StitchColor { get; private set; }
    
    
    public enum Neighbor
    {
        up, 
        right,
        down,
        left
    }

    public enum NodeCorner
    {
        BottomLeft,
        TopLeft,
        TopRight,
        BottomRight,
    }

    public Stitch(VerletNode[] myCorners, Panel parentPanel)
    {
        _corners[0] = myCorners[0];
        _corners[1] = myCorners[1];
        _corners[2] = myCorners[2];
        _corners[3] = myCorners[3];
        ParentPanel = parentPanel;
        Dimensions = _corners[0].Dimensions;
        StitchColor = Color.black;
    }

    public enum StitchType
    {
        NORMAL,
        DECREASE,
        INCREASE,
        BINDOFF,
        CASTON
    }
    
    public void SetNeighborStitch(Neighbor myNeighbor, Stitch myStitch)
    {
        if (Neighbors[(int)myNeighbor] != null)
        {
            Debug.LogWarning("neighbor is already assigned.");
            return;
        }

        Neighbors[(int)myNeighbor] = myStitch;
    }

    public void UpdatePosition()
    {
        Position = Util.AveragePosition(_corners.Select(item => item.Position).ToArray());
    }

    private void RemoveStitch()
    {
        //remove stitch and solve all edges & neighbor connections
    }

    private void SelectStitch()
    {
        
    }

    private void UseTool(Tool tool)
    {
        //apply tool effect
    }

    public void UpdateNormal()
    {
        var p1 = _corners[0].Position;
        var p2 = _corners[1].Position;
        var p3 = _corners[2].Position;
        // Compute two vectors in the plane
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;

        // Cross product to get the normal
        Vector3 normal = Vector3.Cross(v1, v2);

        // Normalize the normal vector
        normal = Vector3.Normalize(normal);

        Normal = normal;
    }

    public void SetKnit(bool isKnit)
    {
        Knit = isKnit;
    }

    public void SetColor(Color myColor)
    {
        StitchColor = myColor;
    }
    
    public int GetNeighborElasticity()
    {
        var elasticity = 0;
        if (Neighbors[(int)Neighbor.left]?.Knit != Knit)
        {
            elasticity++;
        }

        if (Neighbors[(int)Neighbor.right]?.Knit != 
            Knit)
        {
            elasticity++;
        }

        return elasticity;
    }

    public void SetElasticityFactor(float factor)
    {
        ElasticityFactor = factor;
        _corners[0].ChangeEdgeLength(_corners[3], Dimensions.x * ElasticityFactor);
        _corners[0].ChangeEdgeLength(_corners[2], Util.CalculateDiagonal(Dimensions.x * ElasticityFactor, Dimensions.y));
        _corners[1].ChangeEdgeLength(_corners[3], Dimensions.x * ElasticityFactor);
        _corners[1].ChangeEdgeLength(_corners[3], Util.CalculateDiagonal(Dimensions.x * ElasticityFactor, Dimensions.y));
        foreach (var c in _corners)
        {
            c.SetSize(Dimensions.x * ElasticityFactor, Dimensions.y);
        }
    }
    
    public VerletNode[] GetCorners()
    {
        switch (stitchType)
        {
            case StitchType.NORMAL:
                return new[]
                    { Corners[0], Corners[1], Corners[2], Corners[3]};
            default:
                return new[] { Corners[0], Corners[1], Corners[2], Corners[3] };
        }
    }
}

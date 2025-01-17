using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using Verlet;

public class Stitch
{
    private VerletNode[] _corners = new VerletNode[4];
    public VerletNode[] Corners => _corners;
    public Vector3 Position { get; private set; }
    private List<Stitch> _neighbors;
    public Panel ParentPanel { get; private set; }
    public Vector3 Normal { get; private set; }
    public StitchType stitchType { get; private set; }
    public bool isKnit { get; private set; }

    public Stitch(VerletNode[] myCorners, Panel parentPanel)
    {
        _corners[0] = myCorners[0];
        _corners[1] = myCorners[1];
        _corners[2] = myCorners[2];
        _corners[3] = myCorners[3];
        ParentPanel = parentPanel;
    }

    public enum StitchType
    {
        NORMAL,
        DECREASE,
        INCREASE,
        BINDOFF,
        CASTON
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

    public void CalculateNormal()
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
}

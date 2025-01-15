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

    public Stitch(VerletNode[] myCorners)
    {
        _corners[0] = myCorners[0];
        _corners[1] = myCorners[1];
        _corners[2] = myCorners[2];
        _corners[3] = myCorners[3];
    }

    enum stitchType
    {
        normal,
        decrease,
        increase,
        bindoff,
        caston
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class Stitch
{
    private VerletNode[] _corners = new VerletNode[4];
    private List<Stitch> _neighbors;

    enum stitchType
    {
        normal,
        decrease,
        increase,
        bindoff,
        caston
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

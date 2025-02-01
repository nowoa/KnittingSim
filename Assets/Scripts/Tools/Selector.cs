using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Verlet;

public class Selector : Tool
{
    private Hover hover = GameManager.Instance.Hover;
    public static List<Stitch> SelectedStitches { get; private set; } = new();

    enum Mode
    {
        DEFAULT,
        COLUMN,
        ROW
    }

    private static Mode _selectionMode;

    public override void DefaultBehavior()
    {
        ApplySelection(_selectionMode);
    }

    public override void MainAction()
    {
        _selectionMode = Mode.ROW;
    }

    public override void MainActionEnd()
    {
        _selectionMode = Mode.DEFAULT;
    }

    public override void SecondaryAction()
    {
        _selectionMode = Mode.COLUMN;
    }

    public override void SecondaryActionEnd()
    {
        _selectionMode = Mode.DEFAULT;
    }

    private void ApplySelection(Mode mode)
    {
        var stitch = hover.HoveredStitch;
        if (stitch == null) return;
        switch (mode)
        {
            case Mode.DEFAULT: return;
            case Mode.ROW:
                
                if (!Input.GetKey(KeyCode.LeftControl)) SelectedStitches = new List<Stitch>();
                if (SelectedStitches.Contains(stitch)) return;
                while (stitch.Corners[0].Traverse(VerletNode.Neighbor.left) != null)
                {
                    if (!SelectedStitches.Contains(stitch))SelectedStitches.Add(stitch);
                    stitch = stitch.Corners[0].Traverse(VerletNode.Neighbor.left).ParentStitch;
                }
                if (!SelectedStitches.Contains(stitch))SelectedStitches.Add(stitch);
                while (stitch.Corners[3].Traverse(VerletNode.Neighbor.right) != null)
                {
                    stitch = stitch.Corners[3].ParentStitch;
                    if (!SelectedStitches.Contains(stitch))SelectedStitches.Add(stitch); //ignore the center one since it was already added in the first while loop
                }
                return;
            
            case Mode.COLUMN:
                
                if (!Input.GetKey(KeyCode.LeftControl)) SelectedStitches = new List<Stitch>();
                if (SelectedStitches.Contains(stitch)) return;
                while (stitch.Corners[1].Traverse(VerletNode.Neighbor.up) != null)
                {
                    if (!SelectedStitches.Contains(stitch))SelectedStitches.Add(stitch);
                    stitch = stitch.Corners[1].ParentStitch;
                }
                if (!SelectedStitches.Contains(stitch))SelectedStitches.Add(stitch);
                while (stitch.Corners[0].Traverse(VerletNode.Neighbor.down) != null)
                {
                    stitch = stitch.Corners[0].Traverse(VerletNode.Neighbor.down).ParentStitch;
                    if (!SelectedStitches.Contains(stitch))SelectedStitches.Add(stitch); //ignore the center one since it was already added in the first while loop
                }
                return;
        }
    }
}

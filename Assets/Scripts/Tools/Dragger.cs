using System.Linq;
using UnityEngine;

public class Dragger : Tool
{
    private Hover hover = GameManager.Instance.Hover;
    public override void MainAction()
    {
        hover.SelectNode(true); // set selected node
        Debug.Log("main action");
    }

    public override void MainActionEnd()
    {
        hover.SelectNode(false); // remove selected node
    }

    public override void SecondaryAction()
    {
        if (hover.SelectedNode != null)
        {
            hover.SelectedNode.ToggleAnchored(hover.GetMouseWorldPos());
            return;
        }
        var cachedNode = hover.HoveredNode;
        if (cachedNode == null)
        {
            return;
        }
        cachedNode.ToggleAnchored(hover.GetMouseWorldPos());
    }

    public override void SpecialAction()
    {
        //remove all anchored nodes
        foreach (var panel in GameManager.Instance.Project.GetPanels())
        {
            // Create a copy of the list to avoid modifying it while iterating
            var anchoredNodesCopy = panel.AnchoredNodes.ToList();

            foreach (var n in anchoredNodesCopy)
            {
                n.ToggleAnchored(new Vector3(0, 0, 0));
            }
        }
    }
}

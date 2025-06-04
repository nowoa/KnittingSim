using System.Linq;
using UnityEngine;
using Verlet;

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
        VerletNode nodeToAnchor = hover.SelectedNode;
        if (nodeToAnchor is null)
        {
            nodeToAnchor = hover.HoveredNode;
        }

        if (nodeToAnchor is null) return;
        GameManager.Instance.Project.anchors.ToggleAnchor(nodeToAnchor, nodeToAnchor.Position);
    }

    public override void OnDeactivate()
    {
        hover.SelectNode(false);
    }
}

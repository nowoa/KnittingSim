public class Dragger : Tool
{
    public override void MainAction()
    {
        MouseHover.UpdateSelected();
        if (MouseHover.SelectedNodeIndex >= 0 && MouseHover.SelectedNodeIndex < FabricManager.AllNodes.Count)
        {
            Debug.Log(FabricManager.AllNodes[MouseHover.SelectedNodeIndex].Connection.Count.ToString());
        }

        /*if (_mouseDragger.HoveredStitchIndex != -1)
        {
            if (FabricManager.AllStitches[_mouseDragger.HoveredStitchIndex].Corners.Contains(null))
            {
                Debug.Log("one or more corners missing");
            }

            foreach (var c in FabricManager.AllStitches[_mouseDragger.HoveredStitchIndex].Corners)
            {
                Debug.Log(c.Position);
            }

        }*/
    }

    public override void MainActionEnd()
    {
        MouseHover.SelectedNodeIndex = -1;
    }

    public override void SecondaryAction()
    {
        var cachedIndex = MouseHover.HoveredNodeIndex;
        if (cachedIndex == -1)
        {
            return;
        }
        var cachedHoveredNode = FabricManager.AllNodes[cachedIndex];

        cachedHoveredNode.IsAnchored = !cachedHoveredNode.IsAnchored;
        cachedHoveredNode.AnchoredPos = MouseHover.GetTargetPos();

    }
}

public class SeamTool : Tool
{
    private VerletNode prevNode;
    private bool seamToolActive;
    private bool isFirstSeam;
    private List<VerletNode> seam1;
    private List<VerletNode> seam2;
    public override void DefaultBehavior()
    {
        base.DefaultBehavior();
        if (!seamToolActive)
        {
            return;
        }

        VerletNode node;

        if (MouseHover.HoveredNodeIndex!=-1)
        {
            node = FabricManager.AllNodes[MouseHover.HoveredNodeIndex];
        }
        else return;

        if (ToolUtils.AreEqual(prevNode, node))
        {
            return; 
        }
        
        prevNode = node;
        
        AddOrRemoveNodeToSeam(node);

    }

    private void AddOrRemoveNodeToSeam(VerletNode myNode)
    {
        if (isFirstSeam)
        {
            if (!seam1.Contains(myNode))
            {
                seam1.Add(myNode);
                Debug.Log("node added to seam1");
            }
        }
        else
        {
            if (!seam2.Contains(myNode))
            {
                seam2.Add(myNode);
                Debug.Log("node added to seam2");
            }
        }
    }

    public override void MainAction()
    {
        seamToolActive = true;
        isFirstSeam = true;
        seam1 = new();
    }

    public override void MainActionEnd()
    {
        seamToolActive = false;
    }

    public override void SecondaryAction()
    {
        seamToolActive = true;
        isFirstSeam = false;
        seam2 = new();
    }

    public override void SecondaryActionEnd()
    {
        seamToolActive = false;
    }

    public override void SpecialAction()
    {
        Debug.Log("special action");
        SeamMaker.ConnectSeams(seam1,seam2);
    }
}
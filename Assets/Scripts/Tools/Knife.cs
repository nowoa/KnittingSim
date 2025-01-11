public class Knife : Tool
{
    private bool isCutting;
    public override void DefaultBehavior()
    {
        base.DefaultBehavior();
        if (isCutting)
        {
            var cachedIndex = MouseHover.HoveredStitchIndex;
            if (cachedIndex != -1)
            {
                var mesh = FabricManager.AllStitches[cachedIndex].ParentMesh;
                Cut(cachedIndex);
                if (mesh != null)
                {
                    mesh.UpdateMesh();
                }
                
            }
        }
    }

    public override void MainAction()
    {
        isCutting = true;
    }

    private void Cut(int myIndex)
    {
        
        if (myIndex == -1)
        {
            return;
        }

        FabricManager.AllStitches[myIndex].RemoveStitch();
        //TO DO: if node doesnt have any edges anymore, remove node
        FabricManager.InvokeUpdateSimulation();
    }

    public override void MainActionEnd()
    {
        isCutting = false;
    }
}
public class StitchBrush : Tool
{
    private bool _knitBrush;
    private bool _purlBrush;
    
    public override void DefaultBehavior()
    {
        base.DefaultBehavior();
        if (MouseHover.HoveredStitchIndex == -1) return;
    
        var stitch = FabricManager.AllStitches[MouseHover.HoveredStitchIndex];
        if (_knitBrush)
        {
            ApplyBrushAction(stitch, true);
        }
        else if (_purlBrush)
        {
            ApplyBrushAction(stitch, false);
        }
    }
    
    public override void MainAction()
    {
        _knitBrush = true;
    }
    
    public override void MainActionEnd()
    {
        _knitBrush = false;
    }
    
    public override void SecondaryAction()
    {
        _purlBrush = true;
    }
    
    public override void SecondaryActionEnd()
    {
        _purlBrush = false;
    }
    
    private void ApplyBrushAction(StitchInfo stitch, bool isKnit)
    {
        if (stitch.Knit == isKnit) return;
    
        stitch.Knit = isKnit;
        ApplyElasticityToNeighbors(stitch);
        UpdateParentMesh(stitch);
    }
    
    private void ApplyElasticityToNeighbors(StitchInfo stitch)
    {
        ApplyElasticity(stitch);
        ApplyElasticity(stitch.StitchRight);
        ApplyElasticity(stitch.StitchLeft);
    }
    
    private void ApplyElasticity(StitchInfo stitch)
    {
        if (stitch == null) return;
    
        switch (stitch.GetNeighborElasticity())
        {
            case 0:
                stitch.SetElasticityFactor(1f);
                break;
            case 1:
                stitch.SetElasticityFactor(0.9f);
                break;
            case 2:
                stitch.SetElasticityFactor(0.8f);
                break;
        }
    }
    
    private void UpdateParentMesh(StitchInfo stitch)
    {
        var parentMesh = stitch.ParentMesh;
        if (parentMesh != null)
        {
            parentMesh.UpdateMesh();
        }
    }
}
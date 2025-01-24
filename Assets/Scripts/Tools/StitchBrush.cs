using static Stitch.Neighbor;

public class StitchBrush : Tool
{
    private bool _knitBrush;
    private bool _purlBrush;
    private Hover hover = GameManager.Instance.Hover;
    public override void DefaultBehavior()
    {
        base.DefaultBehavior();
        if (hover.HoveredStitch == null) return;

        var stitch = hover.HoveredStitch;
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
    
    private void ApplyBrushAction(Stitch myStitch, bool isKnit)
    {
        if (myStitch.Knit == isKnit) return;
    
        myStitch.SetKnit(isKnit);
        ApplyElasticityToNeighbors(myStitch);
        GameManager.Instance.EventManager.InvokeStructureUpdate();
    }
    
    private void ApplyElasticityToNeighbors(Stitch myStitch)
    {
        ApplyElasticity(myStitch);
        ApplyElasticity(myStitch.Neighbors[(int)right]);
        ApplyElasticity(myStitch.Neighbors[(int)left]);
    }
    
    private void ApplyElasticity(Stitch myStitch)
    {
        if (myStitch == null) return;
    
        switch (myStitch.GetNeighborElasticity())
        {
            case 0:
                myStitch.SetElasticityFactor(1f);
                break;
            case 1:
                myStitch.SetElasticityFactor(0.9f);
                break;
            case 2:
                myStitch.SetElasticityFactor(0.8f);
                break;
        }
    }
}
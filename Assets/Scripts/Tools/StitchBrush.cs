using Unity.VisualScripting.FullSerializer.Internal.Converters;
using UnityEditorInternal;
using static Stitch.Neighbor;

public class StitchBrush : Tool
{
    private Hover hover = GameManager.Instance.Hover;

    enum ActiveBrush
    {
        NONE,
        KNIT,
        PURL
    }

    private ActiveBrush _activeBrush;
    public override void DefaultBehavior()
    {
        if (hover.StitchesInRadius.Count == 0) return;
        
        if(_activeBrush == ActiveBrush.NONE) return;

        bool isKnit = _activeBrush == ActiveBrush.KNIT;
        
        foreach (var stitch in hover.StitchesInRadius)
        {
            ApplyBrushAction(stitch, isKnit);
        }
        GameManager.Instance.EventManager.InvokeStructureUpdate();
    }
    
    public override void MainAction()
    {
        _activeBrush = ActiveBrush.KNIT;
    }
    
    public override void MainActionEnd()
    {
        _activeBrush = ActiveBrush.NONE;
    }
    
    public override void SecondaryAction()
    {
        _activeBrush = ActiveBrush.PURL;
    }
    
    public override void SecondaryActionEnd()
    {
        _activeBrush = ActiveBrush.NONE;
    }
    
    private void ApplyBrushAction(Stitch myStitch, bool isKnit)
    {
        if (myStitch.Knit == isKnit) return;
    
        myStitch.SetKnit(isKnit);
        ApplyElasticityToNeighbors(myStitch);
        
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
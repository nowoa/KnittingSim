using UnityEngine;

public class ColorBrush : Tool
{
    private Hover hover = GameManager.Instance.Hover;
    private bool active = false;

    public override void DefaultBehavior()
    {
        if (!active) return;
        if (hover.StitchesInRadius.Count == 0) return;
        
        foreach (var stitch in hover.StitchesInRadius)
        {
            ApplyBrushAction(stitch, GameManager.Instance.ColorPalette.currentColor);
            
        }
        
        GameManager.Instance.EventManager.InvokeStructureUpdate();
    }
    
    public override void MainAction()
    {
        active = true;
        Debug.Log("main action");
    }
    
    public override void MainActionEnd()
    {
        active = false;
    }
    
    public override void SecondaryAction()
    {
    }
    
    public override void SecondaryActionEnd()
    {
    }
    
    private void ApplyBrushAction(Stitch myStitch, Color myColor)
    {
        myStitch.SetColor(myColor);
    }
}
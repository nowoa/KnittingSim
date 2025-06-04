using UnityEngine;

public class ColorBrush : Tool
{
    private Hover hover = GameManager.Instance.Hover;
    private bool active = false;

    public override void DefaultBehavior()
    {
        if (!active) return;
        if (hover.IndicesInRadius.Count == 0) return;

        foreach (var index in hover.IndicesInRadius)
        {
            ApplyBrushAction(GameManager.Instance.ColorPalette.currentColor, index);
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
    
    private void ApplyBrushAction(Color myColor, int index)
    {
        Random.InitState(index);
        GameManager.Instance.Project.Stitches[index].SetColor(myColor + new Color((0.5f - Random.value)/10, (0.5f - Random.value)/10, (0.5f - Random.value)/10));
        // add this to add a little bit of randomisation to the color -- causes it to flicker if you hold the mouse in the same pos
        //  + new Color((0.5f - Random.value)/20, (0.5f - Random.value)/20, (0.5f - Random.value)/20) 
    }

    public override void OnDeactivate()
    {
        active = false;
    }
}
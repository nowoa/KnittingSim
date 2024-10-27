using System.ComponentModel;
using UnityEditor.UIElements;
using UnityEngine;
using Verlet;

public abstract class Tool
{
    protected MouseDragger _mouseDragger = MouseDragger.Instance;
    public static VerletNode closestNode;
        
    public virtual void DefaultBehavior()
    {
        //hovering
        if (FabricManager.AllNodes != null)
        {
            _mouseDragger.UpdateHover(FabricManager.AllNodes); //TO DO: cache getallnodes, update value when a change is applied
        }
    }

    public virtual void MainAction()
    {
        Debug.Log("No main action implemented");
    }

    public virtual void MainActionEnd()
    {
        Debug.Log("No main action end implemented");
    }

    public virtual void SecondaryAction()
    {
        Debug.Log("No secondary action implemented");
    }

    public virtual void SecondaryActionEnd()
    {
        Debug.Log("No secondary action end implemented");
    }


}

public class Dragger : Tool
{

    public override void MainAction()
    {
        Debug.Log("dragger active");
        _mouseDragger.UpdateSelected();
    }

    public override void MainActionEnd()
    {
        _mouseDragger.SelectedChildIndex = -1;
    }
}

public class StitchBrush : Tool
{
        
    public override void MainAction()
    {
        //apply selected stitch type
    }

        
}

public class Increaser : Tool
{

    public override void MainAction()
    {
        //add stitch at position
    }
}

public class Decreaser : Tool
{

    public override void MainAction()
    {
        //decrease stitch at position
    }
}

public class PanelStamp : Tool
{
        
        
    public override void MainAction()
    {
        //apply panel (maybe add scaling options? aka dragging over a grid to repeat the panel )
    }
}

public class SeamMaker : Tool
{
    public override void MainAction()
    {
        throw new System.NotImplementedException();
    }

    public override void MainActionEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void SecondaryAction()
    {
        throw new System.NotImplementedException();
    }

    public override void SecondaryActionEnd()
    {
        throw new System.NotImplementedException();
    }
}

public static class ToolManager
{
    private static Tool _activeTool;
    public static Tool Dragger = new Dragger();
    public static Tool StitchBrush = new StitchBrush();
    public static Tool Increaser = new Increaser();
    public static Tool Decreaser = new Decreaser();
    public static Tool PanelStamp = new PanelStamp();
    public static Tool SeamMaker = new SeamMaker();


    static ToolManager()
    {
        _activeTool = Dragger;
    }
    
    public static void SetActiveTool(Tool myTool)
    {
        Debug.Log("set active tool:" + myTool.ToString());
        _activeTool = myTool;
    }

    public static void OnDefaultBehavior()
    {
        _activeTool.DefaultBehavior();
    }
    public static void OnMainAction()
    {
        _activeTool.MainAction();
    }

    public static void OnMainActionEnd()
    {
        _activeTool.MainActionEnd();
    }
    public static void OnSecondaryAction()
    {
        _activeTool.SecondaryAction();
    }

    public static void OnSecondaryActionEnd()
    {
        _activeTool.SecondaryActionEnd();
    }
}
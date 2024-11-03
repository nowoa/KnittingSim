using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            _mouseDragger.UpdateHover(FabricManager.AllNodes);
        }

        if (FabricManager.AllStitches != null)
        {
            _mouseDragger.UpdateHoverStitch();
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
        _mouseDragger.UpdateSelected();
    }

    public override void MainActionEnd()
    {
        _mouseDragger.SelectedChildIndex = -1;
    }

    public override void SecondaryAction()
    {
        var cachedIndex = _mouseDragger.HoveredChildIndex;
        if (cachedIndex == -1)
        {
            return;
        }
        var cachedHoveredNode = FabricManager.AllNodes[cachedIndex];

        cachedHoveredNode.IsAnchored = !cachedHoveredNode.IsAnchored;
        cachedHoveredNode.AnchoredPos = _mouseDragger.GetTargetPos();

    }
}

public class StitchBrush : Tool
{
}

public class Increaser : Tool
{
}

public class Decreaser : Tool
{
}

public class PanelStamp : Tool
{
}

public class SeamMaker : Tool
{
}

public class Knife : Tool
{
    private bool isCutting;
    public override void DefaultBehavior()
    {
        base.DefaultBehavior();
        if (isCutting)
        {
            var cachedIndex = _mouseDragger.HoveredStitchIndex;
            if (cachedIndex != -1)
            {
                Cut(cachedIndex);
            }
        }
    }

    public override void MainAction()
    {
        var cachedIndex = _mouseDragger.HoveredStitchIndex;
        Cut(cachedIndex);
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

public static class ToolManager
{
    private static Tool _activeTool;
    public static Tool Dragger = new Dragger();
    public static Tool StitchBrush = new StitchBrush();
    public static Tool Increaser = new Increaser();
    public static Tool Decreaser = new Decreaser();
    public static Tool PanelStamp = new PanelStamp();
    public static Tool SeamMaker = new SeamMaker();
    public static Tool Knife = new Knife();

    static ToolManager()
    {
        _activeTool = Dragger;
    }

    public static void SetActiveTool(Tool myTool)
    {
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
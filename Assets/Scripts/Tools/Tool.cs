using UnityEngine;

public abstract class Tool
{
    //get mouse hoverer

    public virtual void DefaultBehavior()
    {
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

    public virtual void SpecialAction()
    {
        Debug.Log("No special action implemented");
    }
}


public static class ToolManager
{
    public static Tool ActiveTool { get; private set; }
    public static Tool DraggerInstance = new Dragger();
    public static Tool StitchBrushInstance = new StitchBrush();/*
    public static Tool Increaser = new Increaser();
    public static Tool Decreaser = new Decreaser();
    public static Tool PanelStamp = new PanelStamp();
    public static Tool SeamTool = new SeamTool();
    public static Tool Knife = new Knife();*/

    static ToolManager()
    {
        ActiveTool = DraggerInstance;
    }

    public static void SetActiveTool(Tool myTool)
    {
        ActiveTool = myTool;
    }

    public static void OnDefaultBehavior()
    {
        ActiveTool.DefaultBehavior();
    }

    public static void OnMainAction()
    {
        ActiveTool.MainAction();
    }

    public static void OnMainActionEnd()
    {
        ActiveTool.MainActionEnd();
    }

    public static void OnSecondaryAction()
    {
        ActiveTool.SecondaryAction();
    }

    public static void OnSecondaryActionEnd()
    {
        ActiveTool.SecondaryActionEnd();
    }

    public static void OnSpecialAction()
    {
        ActiveTool.SpecialAction();
    }
}


using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
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
        if (_mouseDragger.SelectedChildIndex >= 0 && _mouseDragger.SelectedChildIndex < FabricManager.AllNodes.Count)
        {
            Debug.Log(FabricManager.AllNodes[_mouseDragger.SelectedChildIndex].Connection.Count.ToString());
        }
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
    private List<StitchInfo> stitchesToDecrease;
    private bool toolActivated;
    private bool rightDirection;
    private StitchInfo previousStitchInfo;
    private bool hasExecutedThisFrame;
    private List<StitchInfo> stitchesInDecrease;

    public override void DefaultBehavior()
    {
        
        base.DefaultBehavior();
        if (!toolActivated)
        {
            return;
        }

        var stitchInfo = FabricManager.AllStitches[_mouseDragger.HoveredStitchIndex];

        if (!HasStitchInfoChanged(stitchInfo))
        {
            return; 
        }
        
        bool HasStitchInfoChanged(StitchInfo currentStitchInfo)
        {
            return previousStitchInfo != currentStitchInfo;
        }

        hasExecutedThisFrame = false;
        previousStitchInfo = stitchInfo;

        if (stitchesToDecrease.Count == 0)
        {
            if (stitchInfo.stitchType == StitchInfo.StitchType.DecreaseFirst
                || stitchInfo.stitchType == StitchInfo.StitchType.DecreaseMiddle
                || stitchInfo.stitchType == StitchInfo.StitchType.DecreaseLast)
            {
                var stitch = stitchInfo;
                while (stitch.stitchType!=StitchInfo.StitchType.DecreaseFirst)
                {
                    stitch = stitch.StitchLeft;
                }

                stitchInfo = stitch;

                while (stitch.stitchType!=StitchInfo.StitchType.DecreaseLast)
                {
                    stitchesInDecrease.Add(stitch);
                    stitch = stitch.StitchRight;
                }
                stitchesInDecrease.Add(stitch);
            }

            if (!stitchesToDecrease.Contains(stitchInfo))
            {
                stitchesToDecrease.Add(stitchInfo);
            }
        }
        if (stitchInfo.corners[0] == stitchesToDecrease.Last().corners[3])
        {
            AddOrRemoveDecrease(stitchInfo, true);
        }
        
        if (stitchInfo.corners[3] == stitchesToDecrease.Last().corners[0])
        {
            AddOrRemoveDecrease(stitchInfo,false);
        }
    }
    
    private void AddOrRemoveDecrease(StitchInfo stitchInfo, bool isRightDirection)
    {
        if (stitchesToDecrease.Count == 1)
        {
            rightDirection = isRightDirection;
        }

        if (rightDirection && isRightDirection)
        {
            if (stitchesInDecrease.Count > 0)
            {
                foreach (var s in stitchesInDecrease)
                {
                    if (!stitchesToDecrease.Contains(s))
                    {
                        stitchesToDecrease.Add(s);
                    }
                }
                stitchesInDecrease.Clear();
            }
            if (!stitchesToDecrease.Contains(stitchInfo))
            {
                stitchesToDecrease.Add(stitchInfo);
            }

            if (stitchInfo.stitchType == StitchInfo.StitchType.DecreaseFirst)
            {
                var stitch = stitchInfo;
                while (stitch.stitchType != StitchInfo.StitchType.DecreaseLast)
                {
                    if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
                    stitch = stitch.StitchRight;
                }
                if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
            }
        }

        if (!rightDirection && !isRightDirection)
        {
            if (stitchesInDecrease.Count > 0)
            {
                for (int i = stitchesInDecrease.Count-1; i >= 0; i--)
                {
                    if(!stitchesToDecrease.Contains(stitchesInDecrease[i])) stitchesToDecrease.Add(stitchesInDecrease[i]);
                }
                stitchesInDecrease.Clear();
            }
            if (!stitchesToDecrease.Contains(stitchInfo))
            {
                stitchesToDecrease.Add(stitchInfo);
            }

            if (stitchInfo.stitchType == StitchInfo.StitchType.DecreaseLast)
            {
                var stitch = stitchInfo;
                while (stitch.stitchType != StitchInfo.StitchType.DecreaseFirst)
                {
                    if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
                    stitch = stitch.StitchLeft;
                }
                if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
            }
        }

        if (((rightDirection && !isRightDirection)|| (!rightDirection && isRightDirection))&& !hasExecutedThisFrame)
        {
            stitchesToDecrease.Remove(stitchesToDecrease.Last());
            hasExecutedThisFrame = true;
        }
        
        Debug.Log(stitchesToDecrease.Count);
    }

    public override void MainAction()
    {
        toolActivated = true;
        stitchesToDecrease = new List<StitchInfo>();
        stitchesInDecrease = new List<StitchInfo>();
        previousStitchInfo = null;
    }

    public override void MainActionEnd()
    {
        toolActivated = false;
        StitchInfo.Decrease(stitchesToDecrease,rightDirection);
        FabricManager.InvokeUpdateSimulation();
    }
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
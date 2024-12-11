using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using Verlet;

public abstract class Tool
{
    protected MouseHover MouseHover = MouseHover.Instance;
    public static VerletNode closestNode;

    public virtual void DefaultBehavior()
    {
        //hovering

        if (FabricManager.AllStitches != null)
        {
            MouseHover.UpdateHoverStitch();
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

    public virtual void SpecialAction()
    {
        Debug.Log("No special action implemented");
    }
}

public class Dragger : Tool
{
    public override void MainAction()
    {
        MouseHover.UpdateSelected();
        if (MouseHover.SelectedNodeIndex >= 0 && MouseHover.SelectedNodeIndex < FabricManager.AllNodes.Count)
        {
            Debug.Log(FabricManager.AllNodes[MouseHover.SelectedNodeIndex].Connection.Count.ToString());
        }

        /*if (_mouseDragger.HoveredStitchIndex != -1)
        {
            if (FabricManager.AllStitches[_mouseDragger.HoveredStitchIndex].Corners.Contains(null))
            {
                Debug.Log("one or more corners missing");
            }

            foreach (var c in FabricManager.AllStitches[_mouseDragger.HoveredStitchIndex].Corners)
            {
                Debug.Log(c.Position);
            }
            
        }*/
    }

    public override void MainActionEnd()
    {
        MouseHover.SelectedNodeIndex = -1;
    }

    public override void SecondaryAction()
    {
        var cachedIndex = MouseHover.HoveredNodeIndex;
        if (cachedIndex == -1)
        {
            return;
        }
        var cachedHoveredNode = FabricManager.AllNodes[cachedIndex];

        cachedHoveredNode.IsAnchored = !cachedHoveredNode.IsAnchored;
        cachedHoveredNode.AnchoredPos = MouseHover.GetTargetPos();

    }
}

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

        StitchInfo stitchInfo = null;
        if (MouseHover.HoveredStitchIndex >= 0 && MouseHover.HoveredStitchIndex < FabricManager.AllStitches.Count)
        {
            stitchInfo = FabricManager.AllStitches[MouseHover.HoveredStitchIndex];
        }

        if (stitchInfo == null)
        {
            return;
        }

        if (ToolUtils.AreEqual(previousStitchInfo,stitchInfo))
        {
            return; 
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
        if (stitchInfo.Corners[0] == stitchesToDecrease.Last().Corners[3])
        {
            AddOrRemoveDecrease(stitchInfo, true);
        }
        
        if (stitchInfo.Corners[3] == stitchesToDecrease.Last().Corners[0])
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
        if (stitchesToDecrease.Count >= 2)
        {
            var decrease = new DecreaseInfo(stitchesToDecrease.First(), stitchesToDecrease.Last(), rightDirection);
            Decrease.Main(decrease);
            FabricManager.InvokeUpdateSimulation();
            var mesh = FabricManager.AllStitches[MouseHover.HoveredStitchIndex].ParentMesh;
            if (mesh != null)
            {
                mesh.UpdateMesh();
            }
            
        }
        
    }
}

public class PanelStamp : Tool
{
}

public class SeamTool : Tool
{
    private VerletNode prevNode;
    private bool seamToolActive;
    private bool isFirstSeam;
    private List<VerletNode> seam1;
    private List<VerletNode> seam2;
    public override void DefaultBehavior()
    {
        base.DefaultBehavior();
        if (!seamToolActive)
        {
            return;
        }

        VerletNode node;

        if (MouseHover.HoveredNodeIndex!=-1)
        {
            node = FabricManager.AllNodes[MouseHover.HoveredNodeIndex];
        }
        else return;

        if (ToolUtils.AreEqual(prevNode, node))
        {
            return; 
        }
        
        prevNode = node;
        
        AddOrRemoveNodeToSeam(node);

    }

    private void AddOrRemoveNodeToSeam(VerletNode myNode)
    {
        if (isFirstSeam)
        {
            if (!seam1.Contains(myNode))
            {
                seam1.Add(myNode);
                Debug.Log("node added to seam1");
            }
        }
        else
        {
            if (!seam2.Contains(myNode))
            {
                seam2.Add(myNode);
                Debug.Log("node added to seam2");
            }
        }
    }

    public override void MainAction()
    {
        seamToolActive = true;
        isFirstSeam = true;
        seam1 = new();
    }

    public override void MainActionEnd()
    {
        seamToolActive = false;
    }

    public override void SecondaryAction()
    {
        seamToolActive = true;
        isFirstSeam = false;
        seam2 = new();
    }

    public override void SecondaryActionEnd()
    {
        seamToolActive = false;
    }

    public override void SpecialAction()
    {
        Debug.Log("special action");
        SeamMaker.ConnectSeams(seam1,seam2);
    }
}

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

public static class ToolManager
{
    private static Tool _activeTool;
    public static Tool Dragger = new Dragger();
    public static Tool StitchBrush = new StitchBrush();
    public static Tool Increaser = new Increaser();
    public static Tool Decreaser = new Decreaser();
    public static Tool PanelStamp = new PanelStamp();
    public static Tool SeamTool = new SeamTool();
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

    public static void OnSpecialAction()
    {
        _activeTool.SpecialAction();
    }
}

public static class ToolUtils
{
    public static bool AreEqual<T>(T prev, T check) 
    {
        return Equals(prev, check);
    }
}


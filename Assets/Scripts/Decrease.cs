using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

public static class Decrease
{
    private static bool _firstDone;
    private static List<DecreaseInfo> _allDecreases;
    
    public static void Main(DecreaseInfo decreaseInfo, bool check = false)
    {
        _allDecreases = new List<DecreaseInfo>();
        if (!decreaseInfo.Direction) 
        {
            //TO DO: if going left, decrease logic needs to be inverted
            //... or the list just has to be inverted?
            return;
        }
        // get list of decreases to perform
        _allDecreases.Add(decreaseInfo);
        CheckForDecreases(decreaseInfo);
        _allDecreases.Reverse();
        foreach (var d in _allDecreases)
        {
            ExecuteDecrease(d);
        }
    }

    private static void ExecuteDecrease(DecreaseInfo decreaseInfo)
    {
        _firstDone = false;
        var toRemove = new List<StitchInfo>(GetColumnsToRemove(decreaseInfo));
        RemoveColumns(toRemove);
        ConnectAllStitches(decreaseInfo);
            
        FabricManager.InvokeUpdateSimulation();
    }

    static void CheckForDecreases(DecreaseInfo originalDecrease)
    {
        var (checkFirst,checkLast) = TryReturnDecreasePair(originalDecrease.FirstStitch, originalDecrease.LastStitch);

        if (!checkFirst.type.HasValue)
        {
            Debug.Log("first null");
        }
        else
        {
            switch (checkFirst.type.Value)
            {
                case StitchInfo.StitchType.DecreaseFirst:
                    Debug.Log("dec started above first");
                    break;
                case StitchInfo.StitchType.DecreaseMiddle:
                    Debug.Log("dec started above middle");
                    break;
                case StitchInfo.StitchType.DecreaseLast:
                    Debug.Log("dec started above last");
                    break;
            }
        }
            
        if (!checkLast.type.HasValue)
        {
            Debug.Log("last null");
        }
        else
        {
            switch (checkLast.type.Value)
            {
                case StitchInfo.StitchType.DecreaseFirst:
                    Debug.Log("dec ended above first");
                    var newDec = new DecreaseInfo(checkFirst.stitch, checkLast.stitch, originalDecrease.Direction);
                    _allDecreases.Add(newDec);
                    CheckForDecreases(newDec);
                    
                    break;
                case StitchInfo.StitchType.DecreaseMiddle:
                    Debug.Log("dec ended above middle");
                    break;
                case StitchInfo.StitchType.DecreaseLast:
                    Debug.Log("dec ended above last");
                    break;
            }
        }
    }

    static ((StitchInfo stitch, StitchInfo.StitchType? type) first, (StitchInfo stitch, StitchInfo.StitchType? type) last) 
        TryReturnDecreasePair(StitchInfo firstStitch, StitchInfo lastStitch)
    {
        var currentFirst = firstStitch.StitchBelow;
        var currentLast = lastStitch.StitchBelow;

        while (currentFirst != null || currentLast != null)
        {
            // Check for decrease type in currentFirst
            if (currentFirst != null && currentFirst.stitchType is StitchInfo.StitchType.DecreaseFirst 
                    or StitchInfo.StitchType.DecreaseMiddle 
                    or StitchInfo.StitchType.DecreaseLast)
            {
                return ((currentFirst, currentFirst.stitchType), (currentLast, null));
            }

            // Check for decrease type in currentLast
            if (currentLast != null && currentLast.stitchType is StitchInfo.StitchType.DecreaseFirst 
                    or StitchInfo.StitchType.DecreaseMiddle 
                    or StitchInfo.StitchType.DecreaseLast)
            {
                return ((currentFirst, null), (currentLast, currentLast.stitchType));
            }

            // Move both stitches downward
            currentFirst = currentFirst?.StitchBelow;
            currentLast = currentLast?.StitchBelow;
        }

        // If no decrease type is found in either chain
        return ((null, null), (null, null));
    }


    static List<StitchInfo> GetColumnsToRemove(DecreaseInfo decreaseInfo)
    {
        var columnsToRemove = new List<StitchInfo>(GetStitchesBetween(decreaseInfo.FirstStitch, decreaseInfo.LastStitch));
        if (columnsToRemove.Last().stitchType == StitchInfo.StitchType.DecreaseLast)
        {
            while (columnsToRemove.Last().stitchType != StitchInfo.StitchType.DecreaseFirst)
            {
                columnsToRemove.Last().SetStitchType(StitchInfo.StitchType.DecreaseMiddle);
                columnsToRemove.Remove(columnsToRemove.Last());
            }
        }
        return columnsToRemove;
    }

    private static void RemoveColumns(List<StitchInfo> toRemove)
    {
        for (int i = 0; i < toRemove.Count-1;i++)
        {
            RemoveColumnRecursive(toRemove[i]);
        }
    }
    public static List<StitchInfo> GetDecreaseStitches(StitchInfo hoveredStitch)
    {
        var stitch = hoveredStitch;
        var decreasedStitchesList = new List<StitchInfo>();
        
        while (stitch.stitchType != StitchInfo.StitchType.DecreaseFirst)
        {
            stitch = stitch.StitchLeft;
        }

        while (stitch.stitchType != StitchInfo.StitchType.DecreaseLast)
        {
            decreasedStitchesList.Add(stitch);
            stitch = stitch.StitchRight;
        }
        decreasedStitchesList.Add(stitch);
        return decreasedStitchesList;
    }

    static List<StitchInfo> GetStitchesBetween(StitchInfo left, StitchInfo right)
    {
        var returnList = new List<StitchInfo>();
        var current = left;
        while (current != right)
        {
            if (!returnList.Contains(current))
            {
                returnList.Add(current);
            }
            current = current.StitchRight;
        }
        returnList.Add(current);
        return returnList;
    }

    private static void RemoveColumnRecursive(StitchInfo myStitch, bool remove = false) 
    {
        if (myStitch.StitchBelow!=null && myStitch.StitchBelow.stitchType is StitchInfo.StitchType.DecreaseFirst
            or StitchInfo.StitchType.DecreaseMiddle or StitchInfo.StitchType.DecreaseLast)
        {
            FabricManager.AllStitches.Remove(myStitch.Corners[3].Parent);
            return;
        }
        myStitch.Corners[3].RemoveAllEdges();
        if (myStitch.StitchBelow != null )
        {
            RemoveColumnRecursive(myStitch.StitchBelow, true); 
        }
        FabricManager.AllNodes.Remove(myStitch.Corners[3]);
        if (remove)
        {
            FabricManager.AllStitches.Remove(myStitch.Corners[3].Parent);
        }
    }
        
    private static void ConnectAllStitches(DecreaseInfo decreaseInfo)
    {
        var currentStitch = decreaseInfo.FirstStitch;
        for (int i = 0; i < decreaseInfo.Size - 1; i++)
        {
            currentStitch = decreaseInfo.Direction ? currentStitch.StitchRight : currentStitch.StitchLeft;
            ConnectStitch(currentStitch, decreaseInfo);
        }
    }
    private static void ConnectStitch(StitchInfo stitch, DecreaseInfo decreaseInfo)
    {
        if (!_firstDone)
        {
            ConnectOuterStitches(decreaseInfo);
            _firstDone = true;
        }
        else
        {
            ConnectInnerStitches(stitch, decreaseInfo);
        }
    }

    private static void ConnectOuterStitches(DecreaseInfo decrease)
    {
        var left = decrease.FirstStitch.Corners[0];
        var right = decrease.LastStitch.Corners[3];
        var first = decrease.FirstStitch;
        var last = decrease.LastStitch;
        var diagonalLength = Calculation.CalculateDiagonal((first.width + last.width) / 2,
            (first.height + last.height) / 2);
        if (!_firstDone)
        {
            //right structural
            first.SetStitchType(StitchInfo.StitchType.DecreaseFirst);
            last.SetStitchType(StitchInfo.StitchType.DecreaseLast);
            VerletEdge.ConnectNodes(left, right, last.width, VerletEdge.EdgeType.Structural);
            left.RemoveStructuralEdge( false);
            left.SetStructuralEdge( false);
            
            first.UpdateCorners(right,3);
            last.UpdateCorners(left,0);
            
            //structural up
            VerletEdge.ConnectNodes(left,last.Corners[1],last.height, VerletEdge.EdgeType.Structural);
            /*left.SetStructuralEdge(true);*/
            
            //shear up
            VerletEdge.ConnectNodes(left,first.Corners[2],diagonalLength,VerletEdge.EdgeType.Shear);
            left.RemoveShearEdge(true);
            left.SetShearEdge(true);
            
            //shear down
            VerletEdge.ConnectNodes(first.Corners[1],right,diagonalLength,VerletEdge.EdgeType.Shear);
            first.Corners[1].RemoveShearEdge(false);
            first.Corners[1].SetShearEdge(false);
            
            //shear up
            VerletEdge.ConnectNodes(left,last.Corners[2],diagonalLength,VerletEdge.EdgeType.Shear);
            left.RemoveShearEdge(true);
            left.SetShearEdge(true);
            
            //shear
            VerletEdge.ConnectNodes(last.Corners[1],right,diagonalLength,VerletEdge.EdgeType.Shear);
            last.Corners[1].RemoveShearEdge(false);
            last.Corners[1].SetShearEdge(false);

            if (first.StitchBelow != null && last.StitchBelow!=null)
            {
                ConnectColumnsRecursive(first.StitchBelow, last.StitchBelow);
            }
        }
    }

    private static void ConnectInnerStitches(StitchInfo myStitch, DecreaseInfo decrease)
    {
        var first = decrease.FirstStitch;
        var last = decrease.LastStitch;
        var diagonalLength = Calculation.CalculateDiagonal((myStitch.width + last.width) / 2,
            (myStitch.height + last.height) / 2);
        myStitch.SetStitchType(StitchInfo.StitchType.DecreaseMiddle);
        VerletEdge.ConnectNodes(myStitch.Corners[1],first.Corners[0],myStitch.height, VerletEdge.EdgeType.Structural);
        VerletEdge.ConnectNodes(myStitch.Corners[2],last.Corners[3],myStitch.height, VerletEdge.EdgeType.Structural);
        myStitch.UpdateCorners(first.Corners[0],0);
        myStitch.UpdateCorners(last.Corners[3],3);
        VerletEdge.ConnectNodes(myStitch.Corners[1],myStitch.Corners[3],diagonalLength, VerletEdge.EdgeType.Shear);
        myStitch.Corners[1].RemoveShearEdge(false);
        myStitch.Corners[1].SetShearEdge(false);
        VerletEdge.ConnectNodes(myStitch.Corners[0],myStitch.Corners[2],diagonalLength, VerletEdge.EdgeType.Shear);
    }
    
    private static void ConnectColumnsRecursive(StitchInfo left, StitchInfo right)
    {
        var width = (left.width + right.width) / 2;
        var height = (left.height + right.height) / 2;
        var diagonal = Calculation.CalculateDiagonal(width, height);

        VerletEdge.ConnectNodes(left.Corners[0],right.Corners[3],width, VerletEdge.EdgeType.Structural);
        left.Corners[0].RemoveStructuralEdge(false);
        left.Corners[0].SetStructuralEdge(false);
        
        VerletEdge.ConnectNodes(left.StitchAbove.Corners[0],right.Corners[3],diagonal, VerletEdge.EdgeType.Shear);
        left.StitchAbove.Corners[0].RemoveShearEdge(false);
        left.StitchAbove.Corners[0].SetShearEdge(false);
        
        VerletEdge.ConnectNodes(left.Corners[0],right.StitchAbove.Corners[3],diagonal, VerletEdge.EdgeType.Shear);
        left.Corners[0].RemoveShearEdge(true);
        left.Corners[0].SetShearEdge(true);
        
        left.UpdateCorners(right.Corners[2], 2);
        left.UpdateCorners(right.Corners[3],3);

        SetColumnInactive(left, right);
        
        if (left.StitchBelow != null && right.StitchBelow != null)
        {
            ConnectColumnsRecursive(left.StitchBelow,right.StitchBelow);
        }
    }

    static void SetColumnInactive(StitchInfo left, StitchInfo right)
    {
        right.SetInactive();
        if (left.Corners[3].Parent != null)
        {
            left.UpdateNeighborStitch(left.Corners[3].Parent,"right");
            left.StitchRight.UpdateNeighborStitch(left,"left");
        }
        else
        {
            left.UpdateNeighborStitch(null, "right");
        }
    }
}

public struct DecreaseInfo
{
    public StitchInfo FirstStitch;
    public StitchInfo LastStitch;
    public int Size;
    public bool Direction;

    public DecreaseInfo(StitchInfo firstStitch, StitchInfo lastStitch, bool direction) : this()
    {
        FirstStitch = firstStitch;
        Direction = direction;
        LastStitch = lastStitch;
        GetSize();
    }

    void GetSize()
    {
        var currentStitch = FirstStitch;
        var length = 1;
        while (currentStitch != LastStitch)
        {
            currentStitch = Direction ? currentStitch.StitchRight : currentStitch.StitchLeft;
            length++;
        }

        Size = length;
    }
}

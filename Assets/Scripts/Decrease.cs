using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

public static class Decrease
{
    private static StitchInfo _firstDecrease;
    private static bool _firstDone;
    private static StitchInfo _lastDecrease;
    public static void Main(List<StitchInfo> myStitches, bool myDirection)
    {
        if (!myDirection) 
        {
            //TO DO: if going left, decrease logic needs to be inverted
            return;
        }
            
        InitialiseDecrease(myStitches);
        CheckForDecreases();
        var toRemove = GetColumnsToRemove(myStitches);
        RemoveColumns(toRemove);
        ConnectAllStitches(myStitches);
            
        FabricManager.InvokeUpdateSimulation();
    }

    static void CheckForDecreases()
    {
        var checkFirst = TryReturnDecreaseStitchType(_firstDecrease);
        var checkLast = TryReturnDecreaseStitchType(_lastDecrease);

        if (checkFirst == null)
        {
            Debug.Log("first null");
        }
        else
        {
            switch (checkFirst.Value)
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
        if (checkLast == null)
        {
            Debug.Log("last null");
        }
        else
        {
            switch (checkLast.Value)
            {
                case StitchInfo.StitchType.DecreaseFirst:
                    Debug.Log("dec ended above first");
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

    static StitchInfo.StitchType? TryReturnDecreaseStitchType(StitchInfo stitch)
    {
        var currentStitch = stitch;

        while (currentStitch != null)
        {
            currentStitch = currentStitch.StitchBelow;

            // Check if current stitch is a decrease stitch
            if (currentStitch == null) return null; // No stitch below
            if (currentStitch.stitchType == StitchInfo.StitchType.DecreaseFirst ||
                currentStitch.stitchType == StitchInfo.StitchType.DecreaseMiddle ||
                currentStitch.stitchType == StitchInfo.StitchType.DecreaseLast)
            {
                return currentStitch.stitchType; // Return the type immediately
            }
        }

        return null; // No decrease stitch found
    }

    private static void InitialiseDecrease(List<StitchInfo> myStitches)
    {
        _firstDecrease = myStitches.First();
        _firstDone = false;
        _lastDecrease = myStitches.Last();
    }

    static List<StitchInfo> GetColumnsToRemove(List<StitchInfo> myStitches)
    {
        var columnsToRemove = new List<StitchInfo>(myStitches);
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

    private static void RemoveColumnRecursive(StitchInfo myStitch, bool remove = false) 
    {
        
        myStitch.corners[3].RemoveAllEdges();
        if (myStitch.StitchBelow != null)
        {
            RemoveColumnRecursive(myStitch.StitchBelow, true); 
        }
        FabricManager.AllNodes.Remove(myStitch.corners[3]);
        if (remove)
        {
            FabricManager.AllStitches.Remove(myStitch.corners[3].Parent);
        }
    }
        
    private static void ConnectAllStitches(List<StitchInfo> stitches)
    {
        for (int i = 0; i < stitches.Count - 1; i++)
        {
            ConnectStitch(stitches[i]);
        }
    }
    private static void ConnectStitch(StitchInfo stitch)
    {
        if (!_firstDone)
        {
            ConnectOuterStitches();
            _firstDone = true;
        }
        else
        {
            ConnectInnerStitches(stitch);
        }
    }

    private static void ConnectOuterStitches()
    {
        var left = _firstDecrease.corners[0];
        var right = _lastDecrease.corners[3];
        var diagonalLength = Calculation.CalculateDiagonal((_firstDecrease.width + _lastDecrease.width) / 2,
            (_firstDecrease.height + _lastDecrease.height) / 2);
        if (!_firstDone)
        {
            _firstDecrease.SetStitchType(StitchInfo.StitchType.DecreaseFirst);
            _lastDecrease.SetStitchType(StitchInfo.StitchType.DecreaseLast);
            VerletEdge.ConnectNodes(left, right, _lastDecrease.width);
            left.SetStructuralEdge( false);
            
            _firstDecrease.UpdateCorners(right,3);
            _lastDecrease.UpdateCorners(left,0);
            
            VerletEdge.ConnectNodes(left,_lastDecrease.corners[1],_lastDecrease.height);
            left.SetStructuralEdge(true);
            
            VerletEdge.ConnectNodes(left,_firstDecrease.corners[2],diagonalLength);
            left.SetShearEdge(true);
            
            VerletEdge.ConnectNodes(_firstDecrease.corners[1],right,diagonalLength);
            _firstDecrease.corners[1].SetShearEdge(false);
            
            VerletEdge.ConnectNodes(left,_lastDecrease.corners[2],diagonalLength);
            left.SetShearEdge(true);
            
            VerletEdge.ConnectNodes(_lastDecrease.corners[1],right,diagonalLength);
            _lastDecrease.corners[1].SetShearEdge(false);

            if (_firstDecrease.StitchBelow != null && _lastDecrease.StitchBelow!=null)
            {
                ConnectColumnsRecursive(_firstDecrease.StitchBelow, _lastDecrease.StitchBelow);
            }
        }
    }

    private static void ConnectInnerStitches(StitchInfo myStitch)
    {
        var diagonalLength = Calculation.CalculateDiagonal((myStitch.width + _lastDecrease.width) / 2,
            (myStitch.height + _lastDecrease.height) / 2);
        myStitch.SetStitchType(StitchInfo.StitchType.DecreaseMiddle);
        VerletEdge.ConnectNodes(myStitch.corners[1],_firstDecrease.corners[0],myStitch.height);
        VerletEdge.ConnectNodes(myStitch.corners[2],_lastDecrease.corners[3],myStitch.height);
        myStitch.UpdateCorners(_firstDecrease.corners[0],0);
        myStitch.UpdateCorners(_lastDecrease.corners[3],3);
        VerletEdge.ConnectNodes(myStitch.corners[1],myStitch.corners[3],diagonalLength);
        myStitch.corners[1].SetShearEdge(false);
        VerletEdge.ConnectNodes(myStitch.corners[0],myStitch.corners[2],diagonalLength);
    }
    
    private static void ConnectColumnsRecursive(StitchInfo left, StitchInfo right)
    {
        var width = (left.width + right.width) / 2;
        var height = (left.height + right.height) / 2;
        var diagonal = Calculation.CalculateDiagonal(width, height);

        VerletEdge.ConnectNodes(left.corners[0],right.corners[3],width);
        left.corners[0].SetStructuralEdge(false);
        
        VerletEdge.ConnectNodes(left.StitchAbove.corners[0],right.corners[3],diagonal);
        left.StitchAbove.corners[0].SetShearEdge(false);
        
        VerletEdge.ConnectNodes(left.corners[0],right.StitchAbove.corners[3],diagonal);
        left.corners[0].SetShearEdge(true);
        
        left.UpdateCorners(right.corners[2], 2);
        left.UpdateCorners(right.corners[3],3);

        SetColumnInactive(left, right);
        
        if (left.StitchBelow != null && right.StitchBelow != null)
        {
            ConnectColumnsRecursive(left.StitchBelow,right.StitchBelow);
        }
    }

    static void SetColumnInactive(StitchInfo left, StitchInfo right)
    {
        right.SetInactive();
        if (left.corners[3].Parent != null)
        {
            left.UpdateNeighborStitch(left.corners[3].Parent,"right");
            left.StitchRight.UpdateNeighborStitch(left,"left");
        }
        else
        {
            left.UpdateNeighborStitch(null, "right");
        }
    }
}
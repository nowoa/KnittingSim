using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Verlet;

public static class Decrease
{
    private static StitchInfo _firstDecrease;
    private static bool _firstDone;
    private static StitchInfo _lastDecrease;
    private static bool _direction;
    private static List<StitchInfo> _stitchesToDecrease;
    public static void Main(List<StitchInfo> myStitches, bool myDirection)
    {
        InitialiseDecrease(myStitches, myDirection);
        if (!myDirection) 
        {
            //TO DO: if going left, decrease logic needs to be inverted
            return;
        }
        if (CheckForDecreases())
        {
            return;
        }
        var toRemove = new List<StitchInfo>(GetColumnsToRemove(myStitches));
        RemoveColumns(toRemove);
        ConnectAllStitches(myStitches);
            
        FabricManager.InvokeUpdateSimulation();
    }

    static bool CheckForDecreases()
    {
        var (checkFirst,checkLast) = TryReturnDecreasePair(_firstDecrease, _lastDecrease);

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
            return true;
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
                    List<StitchInfo> decreaseBelow;
                    decreaseBelow = new List<StitchInfo>(GetStitchesBetween(checkFirst.stitch,
                        GetDecreaseStitches(checkLast.stitch).Last()));
                    Main(decreaseBelow, _direction);
                    break;
                case StitchInfo.StitchType.DecreaseMiddle:
                    Debug.Log("dec ended above middle");
                    break;
                case StitchInfo.StitchType.DecreaseLast:
                    Debug.Log("dec ended above last");
                    break;
            }

            return true;
        }

        return false;
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

    private static void InitialiseDecrease(List<StitchInfo> myStitches, bool myDirection)
    {
        _firstDecrease = myStitches.First();
        _firstDone = false;
        _lastDecrease = myStitches.Last();
        _direction = myDirection;
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
            //right structural
            _firstDecrease.SetStitchType(StitchInfo.StitchType.DecreaseFirst);
            _lastDecrease.SetStitchType(StitchInfo.StitchType.DecreaseLast);
            VerletEdge.ConnectNodes(left, right, _lastDecrease.width, VerletEdge.EdgeType.Structural);
            left.RemoveStructuralEdge( false);
            left.SetStructuralEdge( false);
            
            _firstDecrease.UpdateCorners(right,3);
            _lastDecrease.UpdateCorners(left,0);
            
            //structural up
            VerletEdge.ConnectNodes(left,_lastDecrease.corners[1],_lastDecrease.height, VerletEdge.EdgeType.Structural);
            /*left.SetStructuralEdge(true);*/
            
            //shear up
            VerletEdge.ConnectNodes(left,_firstDecrease.corners[2],diagonalLength,VerletEdge.EdgeType.Shear);
            left.RemoveShearEdge(true);
            left.SetShearEdge(true);
            
            //shear down
            VerletEdge.ConnectNodes(_firstDecrease.corners[1],right,diagonalLength,VerletEdge.EdgeType.Shear);
            _firstDecrease.corners[1].RemoveShearEdge(false);
            _firstDecrease.corners[1].SetShearEdge(false);
            
            //shear up
            VerletEdge.ConnectNodes(left,_lastDecrease.corners[2],diagonalLength,VerletEdge.EdgeType.Shear);
            left.RemoveShearEdge(true);
            left.SetShearEdge(true);
            
            //shear
            VerletEdge.ConnectNodes(_lastDecrease.corners[1],right,diagonalLength,VerletEdge.EdgeType.Shear);
            _lastDecrease.corners[1].RemoveShearEdge(false);
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
        VerletEdge.ConnectNodes(myStitch.corners[1],_firstDecrease.corners[0],myStitch.height, VerletEdge.EdgeType.Structural);
        VerletEdge.ConnectNodes(myStitch.corners[2],_lastDecrease.corners[3],myStitch.height, VerletEdge.EdgeType.Structural);
        myStitch.UpdateCorners(_firstDecrease.corners[0],0);
        myStitch.UpdateCorners(_lastDecrease.corners[3],3);
        VerletEdge.ConnectNodes(myStitch.corners[1],myStitch.corners[3],diagonalLength, VerletEdge.EdgeType.Shear);
        myStitch.corners[1].RemoveShearEdge(false);
        myStitch.corners[1].SetShearEdge(false);
        VerletEdge.ConnectNodes(myStitch.corners[0],myStitch.corners[2],diagonalLength, VerletEdge.EdgeType.Shear);
    }
    
    private static void ConnectColumnsRecursive(StitchInfo left, StitchInfo right)
    {
        var width = (left.width + right.width) / 2;
        var height = (left.height + right.height) / 2;
        var diagonal = Calculation.CalculateDiagonal(width, height);

        VerletEdge.ConnectNodes(left.corners[0],right.corners[3],width, VerletEdge.EdgeType.Structural);
        left.corners[0].RemoveStructuralEdge(false);
        left.corners[0].SetStructuralEdge(false);
        
        VerletEdge.ConnectNodes(left.StitchAbove.corners[0],right.corners[3],diagonal, VerletEdge.EdgeType.Shear);
        left.StitchAbove.corners[0].RemoveShearEdge(false);
        left.StitchAbove.corners[0].SetShearEdge(false);
        
        VerletEdge.ConnectNodes(left.corners[0],right.StitchAbove.corners[3],diagonal, VerletEdge.EdgeType.Shear);
        left.corners[0].RemoveShearEdge(true);
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

namespace DefaultNamespace
{
    public static class Decrease
    {
        private static StitchInfo _firstDecrease;
        private static bool _firstDone;
        private static StitchInfo _lastDecrease;
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
    public static void Main(List<StitchInfo> myStitches, bool myDirection)
    {
        _firstDecrease = myStitches.First();
        _firstDone = false;
        var targetStitch = myStitches.Last();
        var removeColumnsList = new List<StitchInfo>(myStitches);
        if (removeColumnsList.Last().stitchType == StitchInfo.StitchType.DecreaseLast)
        {
            while (removeColumnsList.Last().stitchType != StitchInfo.StitchType.DecreaseFirst)
            {
                removeColumnsList.Last().SetStitchType(StitchInfo.StitchType.DecreaseMiddle);
                removeColumnsList.Remove(removeColumnsList.Last());
            }
        }
        if (!myDirection) //TO DO: if going left, decrease logic needs to be inverted
        {
            return;
        }
        for (int i = 0; i < removeColumnsList.Count-1;i++)
        {
            RemoveColumn(removeColumnsList[i]);
        }
        for (int i = 0; i < myStitches.Count-1;i++)
        {
            ConnectDecreasedStitches(myStitches[i],targetStitch);
            if (!_firstDone)
            {
                _firstDone = true;
            }
            if (myStitches.Count > 2 && i<myStitches.Count-2 && myStitches[i].corners[1].BendEdgeHorizontal==null)
            {
                Debug.Log("bend edge connected");
                VerletEdge.ConnectNodes(myStitches[i].corners[1],myStitches[i+2].corners[1], myStitches[i].width*2);
                myStitches[i].corners[1].SetBendEdge(false);
            }
        }
        FabricManager.InvokeUpdateSimulation();
    }
    private static void RemoveColumn(StitchInfo myStitch, bool remove = false) 
    {
        
        myStitch.corners[3].RemoveAllEdges();
        if (myStitch.StitchBelow != null)
        {
            RemoveColumn(myStitch.StitchBelow, true); 
        }
        FabricManager.AllNodes.Remove(myStitch.corners[3]);
        if (remove)
        {
            FabricManager.AllStitches.Remove(myStitch.corners[3].Parent);
        }
    }
    private static void ConnectDecreasedStitches(StitchInfo stitchToConnect, StitchInfo targetStitch)
    {
        var left = _firstDecrease.corners[0];
        var right = targetStitch.corners[3];
        if (!_firstDone)
        {
            _firstDecrease.SetStitchType(StitchInfo.StitchType.DecreaseFirst);
            targetStitch.SetStitchType(StitchInfo.StitchType.DecreaseLast);
            VerletEdge.ConnectNodes(left, right, targetStitch.width);
            left.SetStructuralEdge( false);
            
            _firstDecrease.UpdateCorners(right,3);
            targetStitch.UpdateCorners(left,0);
            
            VerletEdge.ConnectNodes(targetStitch.corners[0],targetStitch.corners[1],targetStitch.height);
            targetStitch.corners[0].SetStructuralEdge(true);
            
            VerletEdge.ConnectNodes(_firstDecrease.corners[0],_firstDecrease.corners[2],Calculation.CalculateDiagonal(_firstDecrease.width,_firstDecrease.height));
            _firstDecrease.corners[0].SetShearEdge(true);
            
            VerletEdge.ConnectNodes(_firstDecrease.corners[1],_firstDecrease.corners[3],Calculation.CalculateDiagonal(_firstDecrease.width, _firstDecrease.height));
            _firstDecrease.corners[1].SetShearEdge(false);
            
            VerletEdge.ConnectNodes(targetStitch.corners[0],targetStitch.corners[2],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            targetStitch.corners[0].SetShearEdge(true);
            
            VerletEdge.ConnectNodes(targetStitch.corners[1],targetStitch.corners[3],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            targetStitch.corners[1].SetShearEdge(false);
            
            
            ConnectColumns(_firstDecrease.StitchBelow,targetStitch.StitchBelow);
        }
        else
        {
            stitchToConnect.SetStitchType(StitchInfo.StitchType.DecreaseMiddle);
            VerletEdge.ConnectNodes(stitchToConnect.corners[1],_firstDecrease.corners[0],stitchToConnect.height);
            VerletEdge.ConnectNodes(stitchToConnect.corners[2],targetStitch.corners[3],stitchToConnect.height);
            stitchToConnect.UpdateCorners(_firstDecrease.corners[0],0);
            stitchToConnect.UpdateCorners(targetStitch.corners[3],3);
            VerletEdge.ConnectNodes(stitchToConnect.corners[1],stitchToConnect.corners[3],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            stitchToConnect.corners[1].SetShearEdge(false);
            VerletEdge.ConnectNodes(stitchToConnect.corners[0],stitchToConnect.corners[2],Calculation.CalculateDiagonal(targetStitch.width,targetStitch.height));
            
        }
    }
    private static void ConnectColumns(StitchInfo left, StitchInfo right)
    {
        var stitchValues = right;
        
        VerletEdge.ConnectNodes(left.corners[0],right.corners[3],stitchValues.width);
        left.corners[0].SetStructuralEdge(false);
        
        VerletEdge.ConnectNodes(left.StitchAbove.corners[0],right.corners[3],Calculation.CalculateDiagonal(stitchValues.width,stitchValues.height));
        left.StitchAbove.corners[0].SetShearEdge(false);
        
        VerletEdge.ConnectNodes(left.corners[0],right.StitchAbove.corners[3],Calculation.CalculateDiagonal(stitchValues.width,stitchValues.height));
        left.corners[0].SetShearEdge(true);

        /*if (left.NodeLeft != null && left.NodeLeft.BendEdgeHorizontal==null)
        {
            VerletEdge.ConnectNodes(left.NodeLeft, right, parent.width * 2);
            left.NodeLeft.SetBendEdge(false);
        }

        if (right.NodeRight != null && left.BendEdgeHorizontal==null)
        {
            VerletEdge.ConnectNodes(left, right.NodeRight, parent.width * 2);
            left.SetBendEdge(false);
        }*/
        
        // TO DO: figure out why the bend edges arent being removed if another decrease is made to the right
        
        left.UpdateCorners(right.corners[2], 2);
        left.UpdateCorners(right.corners[3],3);
        right.SetInactive();
        if (left.corners[3].Parent != null)
        {
            left.UpdateNeighborStitch(left.corners[3].Parent,"right");
            left.StitchRight.UpdateNeighborStitch(left, "left");
        }
        else
        {
            left.UpdateNeighborStitch(null, "right");
        }
        

        if (left.StitchBelow != null && right.StitchBelow != null)
        {
            ConnectColumns(left.StitchBelow,right.StitchBelow);
        }
    }
    }
}
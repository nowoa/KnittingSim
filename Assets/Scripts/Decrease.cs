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
        public static void Main(List<StitchInfo> myStitches, bool myDirection)
        {
            
            if (!myDirection) //TO DO: if going left, decrease logic needs to be inverted
            {
                return;
            }
            
            InitialiseDecrease(myStitches);
            var toRemove = GetColumnsToRemove(myStitches);
            RemoveColumns(toRemove);
            
            for (int i = 0; i < myStitches.Count-1;i++)
            {
                ConnectDecreasedStitches(myStitches[i],_lastDecrease);
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

            if (stitches.Count > 2 && i < stitches.Count - 2)
            {
                ConnectBendEdge(stitches[i], stitches[i + 2]);
            }
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
        if (!_firstDone)
        {
            _firstDecrease.SetStitchType(StitchInfo.StitchType.DecreaseFirst);
            _lastDecrease.SetStitchType(StitchInfo.StitchType.DecreaseLast);
            
            VerletEdge.ConnectNodes(left, right, _lastDecrease.width);
            left.SetStructuralEdge( false);
            
            VerletEdge.ConnectNodes(_lastDecrease.corners[0],_lastDecrease.corners[1], _lastDecrease.height);
            _lastDecrease.corners[0].SetStructuralEdge(true);
            
            _firstDecrease.UpdateCorners(right,3);
            _lastDecrease.UpdateCorners(left,0);
            
            VerletEdge.ConnectNodes(_firstDecrease.corners[0],_firstDecrease.corners[2],Calculation.CalculateDiagonal(_firstDecrease.width,_firstDecrease.height));
            _firstDecrease.corners[0].SetShearEdge(true);
            
            VerletEdge.ConnectNodes(_firstDecrease.corners[1],_firstDecrease.corners[3],Calculation.CalculateDiagonal(_firstDecrease.width, _firstDecrease.height));
            _firstDecrease.corners[1].SetShearEdge(false);
            
            VerletEdge.ConnectNodes(_lastDecrease.corners[0],_lastDecrease.corners[2],Calculation.CalculateDiagonal(_lastDecrease.width,_lastDecrease.height));
            _lastDecrease.corners[0].SetShearEdge(true);
            
            VerletEdge.ConnectNodes(_lastDecrease.corners[1],_lastDecrease.corners[3],Calculation.CalculateDiagonal(_lastDecrease.width,_lastDecrease.height));
            _lastDecrease.corners[1].SetShearEdge(false);

            if (_firstDecrease.StitchBelow != null && _lastDecrease.StitchBelow!=null)
            {
                ConnectColumnsRecursive(_firstDecrease.StitchBelow, _lastDecrease.StitchBelow);
            }
        }
    }

    private static void ConnectInnerStitches(StitchInfo myStitch)
    {
        myStitch.SetStitchType(StitchInfo.StitchType.DecreaseMiddle);
        VerletEdge.ConnectNodes(myStitch.corners[1],_firstDecrease.corners[0],myStitch.height);
        VerletEdge.ConnectNodes(myStitch.corners[2],_lastDecrease.corners[3],myStitch.height);
        myStitch.UpdateCorners(_lastDecrease.corners[0],0);
        myStitch.UpdateCorners(_lastDecrease.corners[3],3);
    }
    
    private static void ConnectColumnsRecursive(StitchInfo left, StitchInfo right)
    {
        var stitchValues = right;

        VerletEdge.ConnectNodes(left.corners[0],right.corners[3],stitchValues.width);
        left.corners[0].SetStructuralEdge(false);
        
        VerletEdge.ConnectNodes(left.StitchAbove.corners[0],right.corners[3],Calculation.CalculateDiagonal(stitchValues.width,stitchValues.height));
        left.StitchAbove.corners[0].SetShearEdge(false);
        
        VerletEdge.ConnectNodes(left.corners[0],right.StitchAbove.corners[3],Calculation.CalculateDiagonal(stitchValues.width,stitchValues.height));
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
    private static void ConnectBendEdge(StitchInfo left, StitchInfo right)
    {
        if (left.corners[1].BendEdgeHorizontal == null)
        {
            Debug.Log("Bend edge connected");
            VerletEdge.ConnectNodes(left.corners[1], right.corners[1], left.width * 2);
            left.corners[1].SetBendEdge(false);
        }
    }

        private static void ConnectDecreasedStitches(StitchInfo stitchToConnect)
        {
            var left = _firstDecrease.corners[0];
            var right = _lastDecrease.corners[3];
            if (!_firstDone)
            {
                _firstDecrease.SetStitchType(StitchInfo.StitchType.DecreaseFirst);
                _lastDecrease.SetStitchType(StitchInfo.StitchType.DecreaseLast);
                VerletEdge.ConnectNodes(left, right, _lastDecrease.width);
                left.SetStructuralEdge( false);
            
                _firstDecrease.UpdateCorners(right,3);
                _lastDecrease.UpdateCorners(left,0);
            
                VerletEdge.ConnectNodes(_lastDecrease.corners[0],_lastDecrease.corners[1],_lastDecrease.height);
                _lastDecrease.corners[0].SetStructuralEdge(true);
            
                VerletEdge.ConnectNodes(_firstDecrease.corners[0],_firstDecrease.corners[2],Calculation.CalculateDiagonal(_firstDecrease.width,_firstDecrease.height));
                _firstDecrease.corners[0].SetShearEdge(true);
            
                VerletEdge.ConnectNodes(_firstDecrease.corners[1],_firstDecrease.corners[3],Calculation.CalculateDiagonal(_firstDecrease.width, _firstDecrease.height));
                _firstDecrease.corners[1].SetShearEdge(false);
            
                VerletEdge.ConnectNodes(_lastDecrease.corners[0],_lastDecrease.corners[2],Calculation.CalculateDiagonal(_lastDecrease.width,_lastDecrease.height));
                _lastDecrease.corners[0].SetShearEdge(true);
            
                VerletEdge.ConnectNodes(_lastDecrease.corners[1],_lastDecrease.corners[3],Calculation.CalculateDiagonal(_lastDecrease.width,_lastDecrease.height));
                _lastDecrease.corners[1].SetShearEdge(false);
            
            
                ConnectColumns(_firstDecrease.StitchBelow,_lastDecrease.StitchBelow);
            }
            else
            {
                stitchToConnect.SetStitchType(StitchInfo.StitchType.DecreaseMiddle);
                VerletEdge.ConnectNodes(stitchToConnect.corners[1],_firstDecrease.corners[0],stitchToConnect.height);
                VerletEdge.ConnectNodes(stitchToConnect.corners[2],_lastDecrease.corners[3],stitchToConnect.height);
                stitchToConnect.UpdateCorners(_firstDecrease.corners[0],0);
                stitchToConnect.UpdateCorners(_lastDecrease.corners[3],3);
                VerletEdge.ConnectNodes(stitchToConnect.corners[1],stitchToConnect.corners[3],Calculation.CalculateDiagonal(_lastDecrease.width,_lastDecrease.height));
                stitchToConnect.corners[1].SetShearEdge(false);
                VerletEdge.ConnectNodes(stitchToConnect.corners[0],stitchToConnect.corners[2],Calculation.CalculateDiagonal(_lastDecrease.width,_lastDecrease.height));
            
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
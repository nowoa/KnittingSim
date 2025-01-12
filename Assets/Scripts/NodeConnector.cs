using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class NodeConnector //handles connecting the nodes to create a panel
{
    public static void ConnectNodes(Panel myPanel)
    {
        var stitchWidth = 10f / myPanel.HorizontalGauge;
        var stitchHeight = 10f / myPanel.VerticalGauge;
        var diagonalLength = Util.CalculateDiagonal(stitchWidth, stitchHeight);
        (float width, float height, float diagonal) dimensions = new(stitchWidth, stitchHeight, diagonalLength);
        
        for (int i = 0; i < myPanel.Nodes.Count; i++)
        {
            int[] neighborIndices = CalculateNeighborIndices(i, myPanel.Width);
            for (var n = 0; n<neighborIndices.Length; n++)
            {
                TryConnectNodes(i, neighborIndices[n], n, dimensions, myPanel);
            }
        }
    }

    private static int[] CalculateNeighborIndices(int i, int myWidth)
    {
        return new int[]
        {
            i + myWidth,     // 0 - up
            i - myWidth,     // 1 - down
            i + 1,           // 2 - right
            i - 1,           // 3 - left
            i - myWidth + 1, // 4 - down right
            i + myWidth + 1, // 6 - up right
            i + 2,           // 7 - bend right
            i + myWidth * 2  // 8 - bend up
        };
    }
    
    private enum Neighbor
    {
        Up,
        Down,
        Right,
        Left,
        DownRight,
        UpRight,
        BendRight,
        BendUp
    }
    
    private static bool IsLastInRow(int index, int width) => (index + 1) % width == 0;
    private static bool IsBeforeLastInRow(int index, int width) => (index + 2) % width == 0;

    private static void TryConnectNodes(int origin, int target, int neighborIndex, (float width, float height, float diagonal) myDimensions, Panel myPanel)
    {
        //check if the connectTo node is within the range
        if (!target.IsInRangeOf(myPanel.Nodes)) return;

        var nodeA = myPanel.Nodes[origin];
        var nodeB = myPanel.Nodes[target];
        
        //switch to determine action based on direction
        switch (neighborIndex)
        {
            case (int)Neighbor.Up: // create edge
                VerletEdge.ConnectNodes(nodeA,nodeB, myDimensions.height, VerletEdge.EdgeType.Structural);
                nodeA.SetNeighborNode(VerletNode.Neighbor.up,nodeB);
                break;
            
            case (int)Neighbor.Down: // set neighbor
                nodeA.SetNeighborNode(VerletNode.Neighbor.down,nodeB);
                break;
            
            case (int)Neighbor.Right: // create edge
                if (IsLastInRow(origin, myPanel.Width)) return;
                VerletEdge.ConnectNodes(nodeA,nodeB,myDimensions.width,VerletEdge.EdgeType.Structural);
                nodeA.SetNeighborNode(VerletNode.Neighbor.right,nodeB);
                break;
            
            case (int)Neighbor.Left: // set neighbor
                if (IsLastInRow(target, myPanel.Width)) return;
                nodeA.SetNeighborNode(VerletNode.Neighbor.left,nodeB);
                break;
            
            case (int)Neighbor.DownRight: // create edge
                if (IsLastInRow(origin, myPanel.Width)) return;
                VerletEdge.ConnectNodes(nodeA,nodeB,myDimensions.diagonal,VerletEdge.EdgeType.Shear);
                break;
            
            case (int)Neighbor.UpRight: // create edge
                if (IsLastInRow(origin, myPanel.Width)) return;
                VerletEdge.ConnectNodes(nodeA,nodeB,myDimensions.diagonal,VerletEdge.EdgeType.Shear);
                break;
            
            case (int)Neighbor.BendRight: // create edge
                if (IsLastInRow(origin, myPanel.Width) || IsBeforeLastInRow(origin, myPanel.Width)) return;
                VerletEdge.ConnectNodes(nodeA,nodeB,myDimensions.width*2,VerletEdge.EdgeType.Bend);
                break;
            
            case (int)Neighbor.BendUp: // create edge
                VerletEdge.ConnectNodes(nodeA,nodeB,myDimensions.height*2,VerletEdge.EdgeType.Bend);
                break;
        }
    }
}
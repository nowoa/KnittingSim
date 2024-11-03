using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

public static class NodeConnector
{
    public static void ConnectNodes(List<VerletNode> myNodes, int myWidth, bool myIsCircular, StitchTemplate myStitchTemplate)
    {
        var diagonalLength = CalculateDiagonal(myStitchTemplate);
        for (int i = 0; i < myNodes.Count; i++)
        {
            
            var diagonalRightUpIndex = i + myWidth + 1;
            var downIndex = i - myWidth;
            var upIndex = i + myWidth;
            var rightIndex = i + 1;
            if (downIndex.IsInRangeOf(myNodes))
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[downIndex],myStitchTemplate.height);
                myNodes[i].AddNodeBelow(myNodes[downIndex]);
                myNodes[downIndex].AddNodeAbove(myNodes[i]);
            }

            var isFirstInRow = i % myWidth == 0;
            var leftIndex = i -1;
            if (!isFirstInRow)
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[leftIndex],myStitchTemplate.width);
                myNodes[i].SetNodeLeft(myNodes[leftIndex]);
                myNodes[leftIndex].SetNodeRight(myNodes[i]);
            }
            
            var diagonalLeftDownIndex = i - myWidth - 1;
            if(!isFirstInRow && diagonalLeftDownIndex.IsInRangeOf(myNodes))
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[diagonalLeftDownIndex],diagonalLength);
            }
            
            
            var isLastInRow = (i + 1) % myWidth == 0;
            var diagonalRightDownIndex = i - myWidth + 1;
            if(!isLastInRow && diagonalRightDownIndex.IsInRangeOf(myNodes))
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[diagonalRightDownIndex],diagonalLength);
            }
            
            var bendEdgeLeftIndex = i - 2;
            var isBeforeFirstInRow = (i - 1) % myWidth == 0;
             if(bendEdgeLeftIndex.IsInRangeOf(myNodes) && !isFirstInRow && !isBeforeFirstInRow)
             {
                 VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeLeftIndex],myStitchTemplate.width*2);
                 myNodes[i].SetHorizontalBendEdge(myNodes[i].Connection.Last());
             }

             var bendEdgeDownIndex = i - myWidth * 2;
             if(bendEdgeDownIndex.IsInRangeOf(myNodes))
             {
                 VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeDownIndex],myStitchTemplate.height*2);
                 myNodes[i].SetVerticalBendEdge(myNodes[i].Connection.Last());
             }
             
            if (myIsCircular)
            {
                var lastNodeIndex = myNodes.Count-1;
                if (i==lastNodeIndex)
                {
                    continue;
                }
                
                
                if (!isLastInRow)
                {
                    continue;
                }
                
                VerletEdge.ConnectNodes(myNodes[i], myNodes[rightIndex],myStitchTemplate.width);
                myNodes[i].SetNodeRight(myNodes[rightIndex]);
                myNodes[rightIndex].SetNodeLeft(myNodes[i]);

                var bendEdgeRightIndex = i + 2;
                if((bendEdgeRightIndex).IsInRangeOf(myNodes))
                {
                    VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeRightIndex],myStitchTemplate.width*2); 
                    myNodes[bendEdgeRightIndex].SetHorizontalBendEdge(myNodes[bendEdgeRightIndex].Connection.Last());
                }
                    
                if ((diagonalRightDownIndex).IsInRangeOf(myNodes))
                {
                    VerletEdge.ConnectNodes(myNodes[i],
                        myNodes[diagonalRightDownIndex],diagonalLength); 
                }

                if ((diagonalRightUpIndex).IsInRangeOf(myNodes))
                {
                    VerletEdge.ConnectNodes(myNodes[i], myNodes[diagonalRightUpIndex],diagonalLength);
                }
            }

            if (diagonalRightUpIndex.IsInRangeOf(myNodes) && !isLastInRow)
            {
                FabricManager.AllStitches.Add(new StitchInfo(myNodes[upIndex],myNodes[diagonalRightUpIndex], myNodes[i], myNodes[rightIndex]));
            }
        }
    }

    private static float CalculateDiagonal(StitchTemplate myStitchTemplate)
    {
        return Mathf.Sqrt(Mathf.Pow(myStitchTemplate.width, 2) + Mathf.Pow(myStitchTemplate.height, 2));
    }
    public static void ConnectSeams(List<VerletNode> seam1, List<VerletNode> seam2)
    {
        if (seam1.Count != seam2.Count)
        {
            Debug.Log("seams are not the same length!" + seam1.Count + seam2.Count);
            return;
        }
        
        for (int i = 0; i < seam1.Count; i++)
        {
            seam1[i].Position += new Vector3(0, 0.5f, 0);
            VerletEdge.ConnectNodes(seam1[i],seam2[i],0.1f);
        }
    }
}

public static class Extensions
{
    public static bool IsInRangeOf<T>(this int index, IList<T> list)
    {
        return index > -1 && index < list.Count;
    }
}

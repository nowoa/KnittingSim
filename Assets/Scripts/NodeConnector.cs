using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;
using static Verlet.VerletNode;

public static class NodeConnector
{
    //TO DO: restructure connections to all use the same directionality (e.g. up and right)
    public static void ConnectNodes(List<VerletNode> myNodes, int myWidth, bool myIsCircular, StitchTemplate myStitchTemplate)
    {
        var diagonalLength = CalculateDiagonal(myStitchTemplate);
        for (int i = 0; i < myNodes.Count; i++)
        {
            
            var diagonalRightUpIndex = i + myWidth + 1;
            var upIndex = i + myWidth;
            var rightIndex = i + 1;
            var isLastInRow = (i + 1) % myWidth == 0;
            var diagonalRightDownIndex = i - myWidth + 1;
            var bendEdgeRightIndex = i + 2;
            var isBeforeLastInRow = (i+2) % myWidth == 0;
            var bendEdgeUpIndex = i + myWidth * 2;
            
            if (upIndex.IsInRangeOf(myNodes)) //structural vertical
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[upIndex],myStitchTemplate.height);
                myNodes[i].SetNodeAbove(myNodes[upIndex]);
                myNodes[upIndex].SetNodeBelow(myNodes[i]);
                myNodes[i].SetStructuralEdge(myNodes[i].Connection.Last(),true);
            }

            if (!isLastInRow) //structural horizontal
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[rightIndex],myStitchTemplate.width);
                myNodes[i].SetNodeRight(myNodes[rightIndex]);
                myNodes[rightIndex].SetNodeLeft(myNodes[i]);
                myNodes[i].SetStructuralEdge(myNodes[i].Connection.Last(),false);
            }
            
            if(!isLastInRow && diagonalRightDownIndex.IsInRangeOf(myNodes)) //shear down
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[diagonalRightDownIndex],diagonalLength);
                myNodes[i].SetShearEdge(myNodes[i].Connection.Last(),false);
            }
            
            if(bendEdgeRightIndex.IsInRangeOf(myNodes) && !isLastInRow && !isBeforeLastInRow) //bend horizontal
            {
                VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeRightIndex],myStitchTemplate.width*2);
                myNodes[i].SetBendEdge(myNodes[i].Connection.Last(),false);
            }

            if(bendEdgeUpIndex.IsInRangeOf(myNodes)) //bend vertical
            {
                VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeUpIndex],myStitchTemplate.height*2);
                myNodes[i].SetBendEdge(myNodes[i].Connection.Last(),true);
            }
            if(!isLastInRow && diagonalRightUpIndex.IsInRangeOf(myNodes)) //shear up
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[diagonalRightUpIndex],diagonalLength);
                myNodes[i].SetShearEdge(myNodes[i].Connection.Last(),true);
                var parentStitch = new StitchInfo(myNodes[i], myNodes[upIndex], myNodes[diagonalRightUpIndex],
                    myNodes[rightIndex]);
                FabricManager.AllStitches.Add(parentStitch);
                myNodes[i].SetParentStitch(parentStitch);
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
                
                VerletEdge.ConnectNodes(myNodes[i], myNodes[rightIndex],myStitchTemplate.width); //structural horizontal
                myNodes[i].SetNodeRight(myNodes[rightIndex]);
                myNodes[rightIndex].SetNodeLeft(myNodes[i]);
                myNodes[i].SetStructuralEdge(myNodes[i].Connection.Last(), false);

                if((bendEdgeRightIndex).IsInRangeOf(myNodes)) //bend horizontal
                {
                    VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeRightIndex],myStitchTemplate.width*2); 
                    myNodes[i].SetBendEdge(myNodes[i].Connection.Last(),false);
                }
                    
                if ((diagonalRightDownIndex).IsInRangeOf(myNodes)) //shear down
                {
                    VerletEdge.ConnectNodes(myNodes[i],
                        myNodes[diagonalRightDownIndex],diagonalLength); 
                    myNodes[i].SetShearEdge(myNodes[i].Connection.Last(),false);
                }

                if ((diagonalRightUpIndex).IsInRangeOf(myNodes)) //shear up
                {
                    VerletEdge.ConnectNodes(myNodes[i], myNodes[diagonalRightUpIndex],diagonalLength);
                    myNodes[i].SetShearEdge(myNodes[i].Connection.Last(),true);
                    var parentStitch = new StitchInfo(myNodes[i], myNodes[upIndex], myNodes[diagonalRightUpIndex],
                        myNodes[rightIndex]);
                    FabricManager.AllStitches.Add(parentStitch);
                    myNodes[i].SetParentStitch(parentStitch);
                }
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
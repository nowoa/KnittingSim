using System.Collections.Generic;
using UnityEngine;
using Verlet;

public static class NodeConnector
{
    public static void ConnectNodes(List<VerletNode> myNodes, int myWidth, bool myIsCircular, StitchTemplate myStitchTemplate)
    {
        var diagonalLength = Calculation.CalculateDiagonal(myStitchTemplate.width, myStitchTemplate.height);
        for (int i = 0; i < myNodes.Count; i++)
        {
            myNodes[i].id = i;
            var diagonalRightUpIndex = i + myWidth + 1;
            var upIndex = i + myWidth;
            var downIndex = i - myWidth;
            var rightIndex = i + 1;
            var leftIndex = i - 1;
            var isLastInRow = (i + 1) % myWidth == 0;
            var diagonalRightDownIndex = i - myWidth + 1;
            var bendEdgeRightIndex = i + 2;
            var isBeforeLastInRow = (i+2) % myWidth == 0;
            var bendEdgeUpIndex = i + myWidth * 2;
            
            if (upIndex.IsInRangeOf(myNodes)) //structural vertical
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[upIndex],myStitchTemplate.height, VerletEdge.EdgeType.Structural);
                myNodes[i].SetStructuralEdge(true);
            }

            if (!isLastInRow) //structural horizontal
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[rightIndex],myStitchTemplate.width, VerletEdge.EdgeType.Structural);
                myNodes[i].SetStructuralEdge(false);
            }
            
            if(!isLastInRow && diagonalRightDownIndex.IsInRangeOf(myNodes)) //shear down
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[diagonalRightDownIndex],diagonalLength, VerletEdge.EdgeType.Shear);
                myNodes[i].SetShearEdge(false);
            }
            
            if(bendEdgeRightIndex.IsInRangeOf(myNodes) && !isLastInRow && !isBeforeLastInRow) //bend horizontal
            {
                VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeRightIndex],myStitchTemplate.width*2, VerletEdge.EdgeType.Bend);
                myNodes[i].SetBendEdge(false);
            }

            if(bendEdgeUpIndex.IsInRangeOf(myNodes)) //bend vertical
            {
                VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeUpIndex],myStitchTemplate.height*2, VerletEdge.EdgeType.Bend);
                myNodes[i].SetBendEdge(true);
            }
            if(!isLastInRow && diagonalRightUpIndex.IsInRangeOf(myNodes)) //shear up
            {
                VerletEdge.ConnectNodes(myNodes[i],myNodes[diagonalRightUpIndex],diagonalLength, VerletEdge.EdgeType.Shear);
                myNodes[i].SetShearEdge(true);
                var parentStitch = new StitchInfo(myNodes[i], myNodes[upIndex], myNodes[diagonalRightUpIndex],
                    myNodes[rightIndex]);
                FabricManager.AllStitches.Add(parentStitch);
                myNodes[i].SetParentStitch(parentStitch);
                if (leftIndex.IsInRangeOf(myNodes)&& myNodes[leftIndex].Parent!=null)
                {
                    parentStitch.UpdateNeighborStitch(myNodes[leftIndex].Parent,"left");
                    myNodes[leftIndex].Parent.UpdateNeighborStitch(parentStitch,"right");
                }

                if(downIndex.IsInRangeOf(myNodes)&& myNodes[downIndex].Parent!=null)
                {
                    parentStitch.UpdateNeighborStitch(myNodes[downIndex].Parent, "below");
                    myNodes[downIndex].Parent.UpdateNeighborStitch(parentStitch,"above");
                }

                myNodes[i].right = myNodes[rightIndex];
                myNodes[i].up = myNodes[upIndex];
                myNodes[rightIndex].left = myNodes[i];
                myNodes[upIndex].down = myNodes[i];

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
                
                VerletEdge.ConnectNodes(myNodes[i], myNodes[rightIndex],myStitchTemplate.width, VerletEdge.EdgeType.Structural); //structural horizontal
                myNodes[i].SetStructuralEdge(false);

                if((bendEdgeRightIndex).IsInRangeOf(myNodes)) //bend horizontal
                {
                    VerletEdge.ConnectNodes(myNodes[i], myNodes[bendEdgeRightIndex],myStitchTemplate.width*2, VerletEdge.EdgeType.Bend); 
                    myNodes[i].SetBendEdge(false);
                }
                    
                if ((diagonalRightDownIndex).IsInRangeOf(myNodes)) //shear down
                {
                    VerletEdge.ConnectNodes(myNodes[i],
                        myNodes[diagonalRightDownIndex],diagonalLength, VerletEdge.EdgeType.Shear); 
                    myNodes[i].SetShearEdge(false);
                }

                if ((diagonalRightUpIndex).IsInRangeOf(myNodes)) //shear up
                {
                    VerletEdge.ConnectNodes(myNodes[i], myNodes[diagonalRightUpIndex],diagonalLength, VerletEdge.EdgeType.Shear);
                    myNodes[i].SetShearEdge(true);
                    var parentStitch = new StitchInfo(myNodes[i], myNodes[upIndex], myNodes[diagonalRightUpIndex],
                        myNodes[rightIndex]);
                    FabricManager.AllStitches.Add(parentStitch);
                    myNodes[i].SetParentStitch(parentStitch);
                }
            }

            
        }
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
            VerletEdge.ConnectNodes(seam1[i],seam2[i],0.1f, VerletEdge.EdgeType.Seam);
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
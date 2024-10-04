using System.Collections.Generic;
using UnityEngine;
using Verlet;

public static class StitchConnector
{
    public static void ConnectStitches(List<StitchScript> stitches, int myWidth, bool isCircular)
    {
        for (int i = 0; i < stitches.Count; i++)
        {
            if (i % myWidth != 0)
            {
                stitches[i].stitchLeft = stitches[i - 1];
                stitches[i - 1].stitchRight = stitches[i];
            }
            //connect vertical
            if (i >= myWidth) //bottom stitch
            {
                stitches[i].stitchBelow = stitches[i - myWidth];
                stitches[i - myWidth].stitchAbove = stitches[i];
            }

            if (isCircular)
            {
                if (i >= stitches.Count - 1)
                {
                    continue;
                }
                
                if (i % myWidth != myWidth - 1)
                {
                    continue;
                }
                stitches[i].stitchRight = stitches[i + 1];
                stitches[i + 1].stitchLeft = stitches[i];
            }
            else
            {
                continue;
            }
            //connect diagonal left & diagonal right
            // connect bend edges (2 left, 2 below)
        }
    }
    
    public static void ConnectNodes(List<VerletNode> nodes, int myWidth, bool isCircular)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (i-myWidth >= 0) // connect node above
            {
                VerletEdge.ConnectNodes(nodes[i],nodes[i-myWidth]);
            }
            if (i % myWidth != 0 && (i-1)>=0) // connect node to the left
            {
                VerletEdge.ConnectNodes(nodes[i],nodes[i-1]);
            }
            if(i-myWidth-1>=0 && i % myWidth != 0) // connect node diagonal left
            {
                VerletEdge.ConnectNodes(nodes[i],nodes[i-myWidth-1]);
            }                   
            if(i-myWidth+1>=0 && (i+1)%myWidth!=0)
            {
                /*connect node diagonal right*/
                VerletEdge.ConnectNodes(nodes[i],nodes[i-myWidth+1]);
            }
                
            if(i-2 >=0 && i % myWidth != 0 && (i-1)%myWidth!=0) // connect node 2 to the left
            {
                VerletEdge.ConnectNodes(nodes[i], nodes[i-2]);
            }
            if(i-myWidth*2 >=0) // connect node 2 up
            {
                VerletEdge.ConnectNodes(nodes[i], nodes[i-myWidth*2]);
            }

            if (isCircular)
            {
                if (i >= nodes.Count - 1)
                {
                    continue;
                }
                
                if (i % myWidth != myWidth - 1)
                {
                    continue;
                }
                VerletEdge.ConnectNodes(nodes[i],nodes[i+1]);
            }
            else
            {
                continue;
            }
        }
    }
}

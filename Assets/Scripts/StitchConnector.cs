using System.Collections.Generic;
public static class StitchConnector
{
    public static void ConnectStitches(List<StitchScript> stitches, int width, bool isCircular)
    {
        for (int i = 0; i < stitches.Count; i++)
        {
            if (i % width != 0)
            {
                stitches[i].stitchLeft = stitches[i - 1];
                stitches[i - 1].stitchRight = stitches[i];
            }
            //connect vertical
            if (i >= width) //bottom stitch
            {
                stitches[i].stitchBelow = stitches[i - width];
                stitches[i - width].stitchAbove = stitches[i];
            }

            if (isCircular)
            {
                if (i >= stitches.Count - 1)
                {
                    return;
                }
                
                if (i % width != width - 1)
                {
                    return;
                }
                stitches[i].stitchRight = stitches[i + 1];
                stitches[i + 1].stitchLeft = stitches[i];
            }
            else
            {
                return;
            }
            //connect diagonal left & diagonal right
            // connect bend edges (2 left, 2 below)
        }
    }
}

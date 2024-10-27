using System.Collections.Generic;
using UnityEngine;

static class GridMaker
{ 
    public static List<StitchInfo> MakePanel(Vector2Int myDimensions)
    {
        List<StitchInfo> stitchInfoList = new List<StitchInfo>();
        
            for (int i = 0; i < myDimensions.y; i++)
            {
                for (int u = 0; u < myDimensions.x; u++)
                {
                    StitchInfo stitch = new StitchInfo(u,i);
                    stitchInfoList.Add(stitch);
                }
            }
        return stitchInfoList;
    }
}
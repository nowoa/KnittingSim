using System.Collections.Generic;
using UnityEngine;

static class GridMaker
{ 
    public static List<StitchInfo> MakePanelWithParameters(Vector2Int myDimensions, StitchTemplate myStitchTemplate, bool myIsCircular)
    {
        List<StitchInfo> stitchInfoList = new List<StitchInfo>();
        var radius = myDimensions.x * myStitchTemplate.width/(2*Mathf.PI);
        float angle = Mathf.PI * 2 / myDimensions.x;
        float row = 0;
        float diagonalIncrement = myStitchTemplate.height /myDimensions.x;
        if (myIsCircular)
        {
            for (var i = 0; i < myDimensions.y; i++)
            {
                for (var u = 0; u < myDimensions.x; u++)
                {
                
                    var newPos = new Vector3(Mathf.Cos(angle * u) * radius, row + diagonalIncrement * u,
                        Mathf.Sin(angle * u) * radius);
                    StitchInfo stitch = new StitchInfo(u,i);
                    stitchInfoList.Add(stitch);
                }
                row+=myStitchTemplate.height;
            }
        }
        else
        {
            for (int i = 0; i < myDimensions.y; i++)
            {
                Vector3 heightPos = new Vector3(0, i*myStitchTemplate.height, 0);
                for (int u = 0; u < myDimensions.x; u++)
                {
                    Vector3 newPos = new Vector3(u * myStitchTemplate.width, heightPos.y, 0);
                    StitchInfo stitch = new StitchInfo(u,i);
                    stitchInfoList.Add(stitch);
                }
            }
        }
        return stitchInfoList;
    }
}
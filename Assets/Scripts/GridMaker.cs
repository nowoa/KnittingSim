using System.Collections.Generic;
using UnityEngine;

static class GridMaker
{ 
    public static List<FabricManager.StitchInfo> MakePanelWithParameters(Vector2Int dimensions, Vector2 stitchPrefabDimensions, bool isCircular)
    {
        List<FabricManager.StitchInfo> stitchInfoList = new List<FabricManager.StitchInfo>();
        var radius = dimensions.x * stitchPrefabDimensions.x/(2*Mathf.PI);
        float angle = Mathf.PI * 2 / dimensions.x;
        float row = 0;
        float diagonalIncrement = stitchPrefabDimensions.y /dimensions.x;
        if (isCircular)
        {
            for (var i = 0; i < dimensions.y; i++)
            {
                for (var u = 0; u < dimensions.x; u++)
                {
                
                    var newPos = new Vector3(Mathf.Cos(angle * u) * radius, row + diagonalIncrement * u,
                        Mathf.Sin(angle * u) * radius);
                    FabricManager.StitchInfo stitch = new FabricManager.StitchInfo(newPos,u,i);
                    stitchInfoList.Add(stitch);
                }
                row+=stitchPrefabDimensions.y;
            }
        }
        else
        {
            for (int i = 0; i < dimensions.y; i++)
            {
                Vector3 heightPos = new Vector3(0, i*stitchPrefabDimensions.y, 0);
                for (int u = 0; u < dimensions.x; u++)
                {
                    Vector3 newPos = new Vector3(u * stitchPrefabDimensions.x, heightPos.y, 0);
                    FabricManager.StitchInfo stitch = new FabricManager.StitchInfo(newPos,u,i);
                    stitchInfoList.Add(stitch);
                }
            }
        }
        return stitchInfoList;
    }
}
/*
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hover
{
    
    private float _hoveredChildDepth;
    public int HoveredNodeIndex;
    public int HoveredStitchIndex;
    private Camera _camera;
    public int SelectedNodeIndex =-1;
  
    
    public void UpdateHoverStitch(List<Stitch> myStitches)
    {
        // Return early if a child is selected
        if (SelectedNodeIndex != -1)
        {
            return;
        }

        HoveredStitchIndex = -1; //reset values
        HoveredNodeIndex = -1;
        
        float closestStitchDistance = float.MaxValue; // Track the closest stitch
        Vector2 normalizedMousePos = NormalizePixelCoords(Input.mousePosition);

        for (var index = 0; index < myStitches.Count; index++)
        {
            var s = myStitches[index];

            // Normalize corner positions and calculate bounding box
            var cornerScreenPos = s.Corners.Select(item => _camera.WorldToScreenPoint(item.Position));
            var cornerPosNormalized = cornerScreenPos.Select(NormalizePixelCoords);
            var posNormalized = cornerPosNormalized as Vector3[] ?? cornerPosNormalized.ToArray();

            float minX = posNormalized.Min(corner => corner.x);
            float maxX = posNormalized.Max(corner => corner.x);
            float minY = posNormalized.Min(corner => corner.y);
            float maxY = posNormalized.Max(corner => corner.y);

            // Check if the mouse is inside the bounding box
            if (normalizedMousePos.x >= minX && normalizedMousePos.x <= maxX &&
                normalizedMousePos.y >= minY && normalizedMousePos.y <= maxY)
            {
                float shortestDistance = float.MaxValue;
                float shortestDistanceC = float.MaxValue;
                
                var screenPoint = _camera.WorldToScreenPoint(s.Position);
                float distance = ((Vector2)NormalizePixelCoords(screenPoint) - normalizedMousePos).magnitude;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                }
                // If this stitch is closer to the mouse than the current closest stitch, update the hovered stitch
                if (shortestDistance < closestStitchDistance)
                {
                    closestStitchDistance = shortestDistance;
                    HoveredStitchIndex = index;
                }

                for (int i = 0; i < posNormalized.Length; i++)
                {
                    float distanceC = ((Vector2)posNormalized[i] - normalizedMousePos).magnitude;
                    if (distanceC < shortestDistanceC)
                    {
                        shortestDistanceC = distanceC;
                        HoveredNodeIndex = FabricManager.AllNodes.IndexOf(s.Corners[i]); //TODO: figure out a way to get the node index without a centralized list of all nodes... maybe by id? or by reference within the hovered stitch?
                        _hoveredChildDepth = posNormalized[i].z;
                    }
                }
            }
        }
    }
    private static Vector3 NormalizePixelCoords(Vector3 pixelCoord)
    {
        float oneOverAverageScreenDimension = 1f / ((Screen.width + Screen.height) / 2f);
        return new Vector3(
            pixelCoord.x * oneOverAverageScreenDimension, 
            pixelCoord.y * oneOverAverageScreenDimension, 
            pixelCoord.z);
    }
    
}
*/

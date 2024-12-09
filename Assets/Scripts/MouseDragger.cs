using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Verlet;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MouseDragger
{
    private static MouseDragger _instance;
    public static MouseDragger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MouseDragger();
            }
            return _instance;
        }
    }
    
    private float _hoveredChildDepth;
    public int HoveredChildIndex;
    public int HoveredStitchIndex;
    private Camera _camera;
    public int SelectedChildIndex =-1;
  
    
    private MouseDragger()
    {
        _camera = Camera.main;
    }
    public void UpdateHoverOld(List<VerletNode> myChildren) // change to take transform instead of verletnode so its reusable
    {
        if (SelectedChildIndex != -1)
        {
            return;
        }

        HoveredChildIndex = -1;
        const float selectionRadius = 0.025f;
        Vector2 normalizedMousePos = NormalizePixelCoords(Input.mousePosition);
        float shortestDistance = float.MaxValue;
        for (var i = 0; i < myChildren.Count; i++)
        {
            var c = myChildren[i];
            Vector3 screenPoint = _camera.WorldToScreenPoint(c.Position);
            Vector2 normalizedChildPos = NormalizePixelCoords(screenPoint);
            var distanceToMouse = (normalizedChildPos - normalizedMousePos).magnitude;
            if (distanceToMouse<selectionRadius && distanceToMouse< shortestDistance)
            {
                HoveredChildIndex = i;
                _hoveredChildDepth = screenPoint.z;
                shortestDistance = distanceToMouse;

            }
        }
    }
    
    public void UpdateHoverStitch()
    {
        var stitches = FabricManager.AllStitches;

        // Return early if a child is selected
        if (SelectedChildIndex != -1)
        {
            return;
        }

        HoveredStitchIndex = -1;
        HoveredChildIndex = -1;
        Vector2 normalizedMousePos = NormalizePixelCoords(Input.mousePosition);

        for (var index = 0; index < stitches.Count; index++)
        {
            var s = stitches[index];

            // Normalize corner positions and calculate bounding box

            var cornerScreenPos = s.Corners.Select(item => _camera.WorldToScreenPoint(item.Position));
            var cornerPosNormalized = cornerScreenPos.Select(item => NormalizePixelCoords(item));
            var posNormalized = cornerPosNormalized as Vector3[] ?? cornerPosNormalized.ToArray();
            
            float minX = posNormalized.Min(corner => corner.x);
            float maxX = posNormalized.Max(corner => corner.x);
            float minY = posNormalized.Min(corner => corner.y);
            float maxY = posNormalized.Max(corner => corner.y);

            // Check if the mouse is inside the bounding box
            if (normalizedMousePos.x >= minX && normalizedMousePos.x <= maxX &&
                normalizedMousePos.y >= minY && normalizedMousePos.y <= maxY)
            {
                HoveredStitchIndex = index;
                
                float shortestDistance = float.MaxValue;

                for (int i = 0; i < posNormalized.Length; i++)
                {
                    float distance = ((Vector2)posNormalized[i] - normalizedMousePos).magnitude;
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        HoveredChildIndex = FabricManager.AllNodes.IndexOf(s.Corners[i]);
                        _hoveredChildDepth = posNormalized[i].z;
                    }
                }
                
                break; // Exit the loop as soon as we find the hovered stitch
            }
        }
        
        
    }

    public void UpdateSelected()
    {
        SelectedChildIndex = HoveredChildIndex;
    }
    
    private static Vector3 NormalizePixelCoords(Vector3 pixelCoord)
    {
        float oneOverAverageScreenDimension = 1f / ((Screen.width + Screen.height) / 2f);
        return new Vector3(
            pixelCoord.x * oneOverAverageScreenDimension, 
            pixelCoord.y * oneOverAverageScreenDimension, 
            pixelCoord.z);
    }

    public Vector3 GetTargetPos()
    {
        Vector3 mousePositionWithDepth = Input.mousePosition + new Vector3(0, 0, _hoveredChildDepth);
        return _camera.ScreenToWorldPoint(mousePositionWithDepth);
    }
    
    public Vector3 MouseToWorldPos(Vector3 myMousePos)
    {
        return _camera.ScreenToWorldPoint(myMousePos);
    }
}

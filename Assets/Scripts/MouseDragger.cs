using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

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
    private float _hoveredStitchDepth;
    public int HoveredChildIndex;
    public int HoveredStitchIndex;
    private Camera _camera;
    public int SelectedChildIndex =-1;
    public int SelectedStitchIndex = -1;
    public List<Vector3> OffsetPositions;
    public float cachedDepth;
  
    
    private MouseDragger()
    {
        _camera = Camera.main;
    }
    public void UpdateHover(List<VerletNode> myChildren) // change to take transform instead of verletnode so its reusable
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
        if (SelectedStitchIndex != -1)
        {
            return;
        }

        HoveredStitchIndex = -1;
        const float selectionRadius = 0.05f;
        Vector2 normalizedMousePos = NormalizePixelCoords(Input.mousePosition);
        float shortestDistance = float.MaxValue;
        for (var i = 0; i < stitches.Count; i++)
        {
            var c = stitches[i];
            Vector3 screenPoint = _camera.WorldToScreenPoint(c.position);
            Vector2 normalizedChildPos = NormalizePixelCoords(screenPoint);
            var distanceToMouse = (normalizedChildPos - normalizedMousePos).magnitude;
            if (distanceToMouse<selectionRadius && distanceToMouse< shortestDistance)
            {
                HoveredStitchIndex = i;
                _hoveredStitchDepth = screenPoint.z;
                shortestDistance = distanceToMouse;

            }
        }
    }

    public void UpdateSelected()
    {
        SelectedChildIndex = HoveredChildIndex;
    }

    public void UpdateSelectedStitch()
    {
        SelectedStitchIndex = HoveredStitchIndex;
        cachedDepth = _hoveredStitchDepth;
        Vector2 normalizedMousePos = NormalizePixelCoords(Input.mousePosition);
        OffsetPositions = new List<Vector3>();
        if (SelectedStitchIndex !=-1)
        {
            foreach (var c in FabricManager.AllStitches[SelectedStitchIndex].corners)
            {
                Vector3 screenPoint = _camera.WorldToScreenPoint(c.Position);
                Vector2 normalizedChildPos = NormalizePixelCoords(screenPoint);
                Vector2 offsetPos = normalizedChildPos - normalizedMousePos;
                OffsetPositions.Add(offsetPos);
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

    public Vector3 GetTargetPos()
    {
        Vector3 mousePositionWithDepth = Input.mousePosition + new Vector3(0, 0, _hoveredChildDepth);
        return _camera.ScreenToWorldPoint(mousePositionWithDepth);
    }

    public Vector3 GetTargetPosStitch()
    {
        Vector3 mousePositionWithDepth = Input.mousePosition + new Vector3(0, 0, cachedDepth);
        return _camera.ScreenToWorldPoint(mousePositionWithDepth);
    }
    
    public Vector3 MouseToWorldPos(Vector3 myMousePos)
    {
        return _camera.ScreenToWorldPoint(myMousePos);
    }
}

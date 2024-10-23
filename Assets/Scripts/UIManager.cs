using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class UIManager
{
    private float _hoveredChildDepth;
    
    // Static instance for the singleton
    private static UIManager _instance;

    // Public property to access the instance
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();  // Create the singleton instance if it doesn't exist
            }
            return _instance;
        }
    }

    // Private constructor to prevent external instantiation
    private UIManager()
    {
        _screenSpacePositions = new List<Vector3>();
        _camera = Camera.main;
    }

    private List<Vector3> _screenSpacePositions;
    private Camera _camera;  // Camera should be passed or set manually
    private Vector3 _selectedStitch;

    // Method to select the closest stitch from a list of stitches
    public int ClosestChild(List<VerletNode> myChildren) // change to return transform instead of verletnode so its reusable
    {
        const float selectionRadius = 0.025f;
        int shortestDistanceIndex = -1;  // Initialize index
        Vector2 normalizedMousePos = NormalizePixelCoords(Input.mousePosition);

        for (var i = 0; i < myChildren.Count; i++)
        {
            var c = myChildren[i];
            // Calculate distance between the mouse position and the current stitch
            float distance = Vector3.Distance(Input.mousePosition, _camera.WorldToScreenPoint(c.position));
            float closestDepth = float.MaxValue;
            Vector3 screenPoint = _camera.WorldToScreenPoint(c.position);
            Vector2 normalizedChildPos = NormalizePixelCoords(screenPoint);
            // Update the closest stitch if the current distance is shorter
            if ((normalizedChildPos-normalizedMousePos).magnitude<selectionRadius && screenPoint.z<closestDepth)
            {
                shortestDistanceIndex = i;
                _hoveredChildDepth = screenPoint.z;
                closestDepth = screenPoint.z;
            }
        }

        return shortestDistanceIndex; // Return the closest stitch
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

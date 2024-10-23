using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

public class MouseDragger
{
    private float _hoveredChildDepth;
    private Camera _camera;
    
    // Private constructor to prevent external instantiation
    public MouseDragger(Camera camera)
    {
        _camera = camera;
    }

    

    // Method to select the closest stitch from a list of stitches

    /*public void GetVectorFromAnything<Anything>(IEnumerable<Anything> items, Func<Anything, Vector3> getter)
    {
        List<Vector3> vectors = new();
        foreach (var item in items)
        {
            vectors.Add(getter(item));
        }
    }
    
    public void CallSillyMethod(List<VerletNode> nodes)
    {
        GetVectorFromAnything(nodes, node => node.position);
        List<Vector3> vectors = nodes.Select(node => node.position).ToList();
    }*/

public int ClosestChild(List<VerletNode> myChildren) // change to take transform instead of verletnode so its reusable
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

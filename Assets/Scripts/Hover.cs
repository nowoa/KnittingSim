using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using Verlet;

public class Hover
{
    
    private float _hoveredChildDepth;
    public VerletNode HoveredNode;
    public Stitch HoveredStitch;
    public VerletNode SelectedNode;


    public (float xMin, float yMin, float xMax, float yMax) PanelBoundingBox(Panel myPanel)
    {
        int interval = Mathf.FloorToInt(myPanel.Width/2)-1;
        while (interval > 5) //at least check 1/nth of nodes (n=5)
        {
            Mathf.FloorToInt(interval /= 2);
        }
        float errorMargin = 0.3f;
        List<Vector3> screenPointNodes = new List<Vector3>();
        foreach (var n in myPanel.Nodes)
        {
            screenPointNodes.Add(GameManager.Instance.Camera.WorldToScreenPoint(n.Position));
        }
        Vector3[] nodesCondensed = new Vector3[screenPointNodes.Count/interval];
        for (int i = 0; i < screenPointNodes.Count / interval; i++)
        {
            nodesCondensed[i] = screenPointNodes[i*interval];
        }
        var boundingBox = BoundingBox(nodesCondensed);
        Debug.Log(boundingBox);
        float widthError = (boundingBox.X.max - boundingBox.X.min) * errorMargin;
        float heightError = (boundingBox.Y.max - boundingBox.Y.min) * errorMargin;
        
        return (boundingBox.X.min - widthError, Screen.height - boundingBox.Y.max - heightError, boundingBox.X.max + widthError, Screen.height -boundingBox.Y.min + heightError);

    }
    
    public void UpdateHoverStitch(List<Stitch> myStitches)
    {
        // Return early if a child is selected
        if (SelectedNode != null)
        {
            return;
        }

        HoveredNode = null;
        HoveredStitch = null;
        
        float closestDistance = float.MaxValue; // Track the closest stitch
        Vector2 mousePos = NormalizePixelCoords(Input.mousePosition);

        foreach (var s in myStitches)
        {
            // Normalize corner positions and calculate bounding box
            var cornerScreenPositions = s.Corners.Select(item => GameManager.Instance.Camera.WorldToScreenPoint(item.Position));
            var positionsNormalized = cornerScreenPositions.Select(NormalizePixelCoords).ToArray();

            if (!InsideBoundingBox(positionsNormalized, mousePos)) continue;
            
            var screenPoint = GameManager.Instance.Camera.WorldToScreenPoint(s.Position);
            float distance = ((Vector2)NormalizePixelCoords(screenPoint) - mousePos).magnitude;
            // If this stitch is closer to the mouse than the current closest stitch, update the hovered stitch
            if (distance < closestDistance)
            {
                closestDistance = distance;
                HoveredStitch = s;
            }
        }

        HoveredNode = GetClosestNode(HoveredStitch, mousePos);
    }

    private VerletNode GetClosestNode(Stitch myStitch, Vector2 myMousePos)
    {
        VerletNode result = null;
        float closestDistance = float.MaxValue;
        foreach (var c in myStitch.Corners)
        {
            var screenPos = GameManager.Instance.Camera.WorldToScreenPoint(c.Position);
            var screenPosNormalized = NormalizePixelCoords(screenPos);
            var distance = ((Vector2)screenPosNormalized - myMousePos).magnitude;
            
            if (!(distance < closestDistance))
            {
                continue;
            }

            closestDistance = distance;
            result = c;
        }

        return result;
    }

    private bool InsideBoundingBox(Vector3[] myPositions, Vector2 myMousePos)
    { // all positions are normalized
        // TODO: convert mypositions to vector2
        var boundingBox = BoundingBox(myPositions);
        
        if (myMousePos.x >= boundingBox.X.min && myMousePos.x <= boundingBox.X.max &&
            myMousePos.y >= boundingBox.Y.min && myMousePos.y <= boundingBox.Y.max)
        {
            return true;
        }
        return false;
    }

    private ((float min, float max) X, (float min, float max) Y) BoundingBox(Vector3[] myPositions)
    {
        float minX = myPositions.Min(position => position.x); 
        float maxX = myPositions.Max(position => position.x);
        float minY = myPositions.Min(position => position.y);
        float maxY = myPositions.Max(position => position.y);

        return ((minX, maxX), (minY, maxY));
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

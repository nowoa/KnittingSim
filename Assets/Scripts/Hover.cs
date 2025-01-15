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



    public void UpdateHover(List<Panel> myPanels)
    {
        HoveredStitch = null;
        HoveredNode = null;
        List<Panel> panelsToCheck = new List<Panel>();
        foreach (var p in myPanels)
        {
            
            if (CheckPanelBoundingBox(p))
            {
                panelsToCheck.Add(p);
            }
        }
        foreach (var p in panelsToCheck)
        {
            Debug.Log(p.Name);
            TrySetHoveredStitch(p.Stitches);
        }
    }

    private bool CheckPanelBoundingBox(Panel myPanel)
    {
        return InsideBoundingBox(PanelBoundingBox(myPanel), Input.mousePosition);
    }
    
    public (Vector2 Min, Vector2 Max) PanelBoundingBox(Panel myPanel)
    {
        int interval = Mathf.FloorToInt(myPanel.Width/2)-1;
        if (interval <= 0) interval = 1;
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
        
        float widthError = (boundingBox.Max.x - boundingBox.Min.x) * errorMargin;
        float heightError = (boundingBox.Max.y - boundingBox.Min.y) * errorMargin;
        
        return (new Vector2(boundingBox.Min.x - widthError,  boundingBox.Min.y - heightError),
            new Vector2(boundingBox.Max.x + widthError, boundingBox.Max.y + heightError));

    }
    
    public (Vector2 Min, Vector2 Max) IMGUIBoundingBox(Panel myPanel)
    {
        var boundingBox = PanelBoundingBox(myPanel);
        return (new Vector2(boundingBox.Min.x, Screen.height - boundingBox.Max.y),
            new Vector2(boundingBox.Max.x, Screen.height - boundingBox.Min.y));
    }
    
    private void TrySetHoveredStitch(List<Stitch> myStitches) //TODO: think about adding error to the stitch bounding boxes?
    {
        Vector2 mousePos = NormalizePixelCoords(Input.mousePosition);
        // Return early if a child is selected
        if (SelectedNode != null)
        {
            return;
        }
        
        float closestDistance = float.MaxValue; // Track the closest stitch
        

        foreach (var s in myStitches)
        {
            // Normalize corner positions and calculate bounding box
            var cornerScreenPositions = s.Corners.Select(item => GameManager.Instance.Camera.WorldToScreenPoint(item.Position));
            var positionsNormalized = cornerScreenPositions.Select(NormalizePixelCoords).ToArray();

            if (!InsideBoundingBox(BoundingBox(positionsNormalized), mousePos)) continue;
            
            var screenPoint = GameManager.Instance.Camera.WorldToScreenPoint(s.Position);
            float distance = ((Vector2)NormalizePixelCoords(screenPoint) - mousePos).magnitude;
            // If this stitch is closer to the mouse than the current closest stitch, update the hovered stitch
            if (distance < closestDistance)
            {
                closestDistance = distance;
                HoveredStitch = s;
            }
        }

        if (HoveredStitch == null) return;
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

    private bool InsideBoundingBox((Vector2 Min, Vector2 Max) bounds, Vector2 myMousePos)
    { 
        
        if (myMousePos.x >= bounds.Min.x && myMousePos.x <= bounds.Max.x &&
            myMousePos.y >= bounds.Min.y && myMousePos.y <= bounds.Max.y)
        {
            return true;
        }
        return false;
    }

    private (Vector2 Min, Vector2 Max) BoundingBox(Vector3[] myPositions)
    {
        float minX = myPositions.Min(position => position.x); 
        float maxX = myPositions.Max(position => position.x);
        float minY = myPositions.Min(position => position.y);
        float maxY = myPositions.Max(position => position.y);

        return (new Vector2(minX,minY), new Vector2(maxX,maxY));
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

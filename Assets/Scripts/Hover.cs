using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using Verlet;

public class Hover
{
    
    public VerletNode HoveredNode;
    public Stitch HoveredStitch;
    public VerletNode SelectedNode;
    private float _selectedNodeDepth;
    private Camera _cam = GameManager.Instance.Camera;
    public bool IsActive = true;
    public List<Stitch> StitchesInRadius;
    public float MouseRadius = 50f; //TODO: turn this into normalized size instead of fixed

    public void UpdateHover(Dictionary<Vector2Int,List<Stitch>> hashGrid)
    {
        HoveredStitch = null;
        HoveredNode = null;
        var mouseCell = SpatialHashGrid.GetCellKey2D(Input.mousePosition, MouseRadius);
        var stitchesToCheck = new List<Stitch>();
        foreach (var o in SpatialHashGrid.offsets2D)
        {
            if (!hashGrid.ContainsKey(mouseCell + o)) continue;
            stitchesToCheck.AddRange(hashGrid[mouseCell+o]);
        }

        StitchesInRadius = CheckRadius(stitchesToCheck, stitch => stitch.Position);
        TrySetHoveredStitch(StitchesInRadius);
    }

    private List<T> CheckRadius<T>(IList<T> items, Func<T, Vector3> PositionGetter)
    {
        var result = new List<T>();
        foreach (var i in items)
        {
            if (DistanceToMousePixels(PositionGetter(i)) > MouseRadius)
            {
                continue;
            }
            result.Add(i);
        }
        return result;
    }

    public void SelectNode(bool state)
    {//dragger calls this method to toggle selected node
        if (state && HoveredNode!=null)
        {
            SelectedNode = HoveredNode;
            _selectedNodeDepth = _cam.WorldToScreenPoint(SelectedNode.Position).z;
        }
        else SelectedNode = null;
    }

    private VerletNode CheckForAnchoredNode(IList<VerletNode> nodes, out bool found)
    {
        found = false;
        var anchoredNodes = nodes.Where(item => item.IsAnchored).ToList();
        if (anchoredNodes.Count == 0) return null;
        var nodesInsideRadius = CheckRadius(anchoredNodes, node => node.Position);
        if (nodesInsideRadius.Count == 0) return null;

        found = true;
        
        float distance;
        float closestDistance = float.MaxValue;
        VerletNode closest = null;
        foreach (var n in anchoredNodes)
        {
            distance = DistanceToMousePixels(_cam.WorldToScreenPoint(n.Position));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = n;
            }
        }

        return closest;
    }
    
    private void TrySetHoveredStitch(List<Stitch> myStitches)
    {
        if (SelectedNode != null) return;
        
        Vector2 mousePos = NormalizePixelCoords(Input.mousePosition);
        float closestDistance = float.MaxValue; // Track the closest stitch

        foreach (var s in myStitches)
        {
            var screenPoint = _cam.WorldToScreenPoint(s.Position);
            float distance = ((Vector2)NormalizePixelCoords(screenPoint) - mousePos).magnitude;
            // If this stitch is closer to the mouse than the current closest stitch, update the hovered stitch
            if (distance < closestDistance)
            {
                closestDistance = distance;
                HoveredStitch = s;
            }
        }

        if (HoveredStitch == null) return;
        
        if (ToolManager.ActiveTool == ToolManager.DraggerInstance)
        {
            var anchoredNode = CheckForAnchoredNode(myStitches.SelectMany(item => item.Corners).ToArray(), out bool found);
            if (found)
            {
                HoveredNode = anchoredNode;
                return;
            }
        }
        HoveredNode = GetClosestNodeFromStitch(HoveredStitch, mousePos);
    }

    private float DistanceToMousePixels(Vector3 worldPos)
    {
        var point = _cam.WorldToScreenPoint(worldPos);
        return (point - Input.mousePosition).magnitude;
    }

    private VerletNode GetClosestNodeFromStitch(Stitch myStitch, Vector2 myMousePos)
    {
        VerletNode result = null;
        float closestDistance = float.MaxValue;
        foreach (var c in myStitch.Corners)
        {
            var screenPos = _cam.WorldToScreenPoint(c.Position);
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

    private static Vector3 NormalizePixelCoords(Vector3 pixelCoord)
    {
        float oneOverAverageScreenDimension = 1f / ((Screen.width + Screen.height) / 2f);
        return new Vector3(
            pixelCoord.x * oneOverAverageScreenDimension, 
            pixelCoord.y * oneOverAverageScreenDimension, 
            pixelCoord.z);
    }

    public Vector3 GetMouseWorldPos()
    {
        Vector3 mousePositionWithDepth = Input.mousePosition + new Vector3(0, 0, _selectedNodeDepth);
        return _cam.ScreenToWorldPoint(mousePositionWithDepth);
    }
}

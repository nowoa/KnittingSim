using System;
using System.Collections.Generic;
using System.Linq;
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
    public bool[] HoverStitchStatus;
    public List<int> IndicesInRadius = new();
    public float MouseRadius = 20f; //TODO: turn this into normalized size instead of fixed

    public void UpdateHover(Dictionary<Vector2Int,List<int>> hashGrid, IList<Vector3> screenPositions, IList<Stitch> stitches)
    {
        HoveredStitch = null;
        HoveredNode = null;
        HoverStitchStatus = new bool[screenPositions.Count];
        IndicesInRadius = new List<int>();
        
        var mouseCell = SpatialHashGrid.GetCellKey2D(Input.mousePosition, MouseRadius);

        int[] indicesToCheck = SpatialHashGrid.offsets2D
            .Where(offset => hashGrid.ContainsKey(offset + mouseCell))
            .SelectMany(offset => hashGrid[offset + mouseCell]).ToArray();
        
        IEnumerable<int> indicesInRange = indicesToCheck.Where(index => ScreenDistance(screenPositions[index], Input.mousePosition) <= MouseRadius);
        
        if (ToolManager.ActiveTool == ToolManager.DraggerInstance)
        {
            SetHoveredState(indicesInRange);
            IEnumerable<int> indicesInRangeSmall =
                indicesToCheck.Where(index => ScreenDistance(screenPositions[index], Input.mousePosition) <= GameManager.Instance.Project.MinimumCellSize);
            //use draggerMouseRadius when dragger is enabled
            TrySetHoveredStitch(indicesInRangeSmall, screenPositions, stitches);
        }
        else
        {
            TrySetHoveredStitch(indicesInRange, screenPositions, stitches);
        }
        
        
    }

    private void SetHoveredState(IEnumerable<int> indicesInRange)
    {
        foreach (var i in indicesInRange)
        {
            HoverStitchStatus[i] = true;
        }
    }

    private List<T> FilterByRadius<T>(IEnumerable<T> items, Func<T, Vector3> PositionGetter, float MouseRadius = 0f)
    {
        if (MouseRadius == 0) MouseRadius = this.MouseRadius;
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

    private VerletNode CheckForAnchoredNode()
    {
        var anchoredNodes = GameManager.Instance.Project.anchors.AnchoredNodes();
        if (anchoredNodes.Length == 0) return null;
        var nodesInsideRadius = FilterByRadius(anchoredNodes, node => node.Position, MouseRadius);
        if (nodesInsideRadius.Count == 0) return null;

        float closestDistance = float.MaxValue;
        VerletNode closest = null;
        foreach (var n in anchoredNodes)
        {
            var distance = DistanceToMousePixels(n.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = n;
            }
        }

        return closest;
    }
    
    private void TrySetHoveredStitch(IEnumerable<int> indexSelection, IList<Vector3> screenPositions, IList<Stitch> stitches)
    {
        if (SelectedNode != null) return;
        foreach (var (key, value) in GameManager.Instance.Project.anchors.GetAnchors())
        {
            value.SetHighlight(0f);
        }
        Vector2 mousePos = NormalizePixelCoords(Input.mousePosition);
        float closestDistance = float.MaxValue; // Track the closest stitch
        
        foreach (int index in indexSelection)
        {
            HoverStitchStatus[index] = true;
            Stitch stitch = stitches[index];
            IndicesInRadius.Add(index);
            var screenPoint = screenPositions[index];
            float distance = ScreenDistance(screenPoint, Input.mousePosition);
            // If this stitch is closer to the mouse than the current closest stitch, update the hovered stitch
            if (distance < closestDistance)
            {
                closestDistance = distance;
                HoveredStitch = stitch;
            }
        }

        if (HoveredStitch == null) return;
        
        if (ToolManager.ActiveTool == ToolManager.DraggerInstance)
        {
            var anchoredNode = CheckForAnchoredNode();
            if (anchoredNode is not null)
            {
                HoveredNode = anchoredNode;
                GameManager.Instance.Project.anchors.GetAnchors()[HoveredNode].SetHighlight(1f);
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

    private float ScreenDistance(Vector3 screenPos1, Vector3 screenPos2)
    {
        Vector2 diff = new Vector2(screenPos2.x - screenPos1.x, screenPos2.y - screenPos1.y);
        return diff.magnitude;
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

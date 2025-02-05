using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public bool[] HoverStitchStatus;
    public List<int> IndicesInRadius = new();
    public float MouseRadius = 20f; //TODO: turn this into normalized size instead of fixed
    private float _cellSize;

    public void UpdateHover(Dictionary<Vector2Int,List<int>> hashGrid, IList<Vector3> screenPositions, IList<Stitch> stitches)
    {
        HoveredStitch = null;
        HoveredNode = null;
        HoverStitchStatus = new bool[screenPositions.Count];
        IndicesInRadius = new List<int>();
        _cellSize = Mathf.Max(MouseRadius, GameManager.Instance.Project.MinimumCellSize);
        
        var mouseCell = SpatialHashGrid.GetCellKey2D(Input.mousePosition, _cellSize);

        IEnumerable<int> indicesToCheck = SpatialHashGrid.offsets2D
            .Where(offset => hashGrid.ContainsKey(offset + mouseCell))
            .SelectMany(offset => hashGrid[offset + mouseCell]).ToArray();

        Vector3 cameraForward = _cam.transform.forward;
        List<int> indicesInViewDir =
            indicesToCheck.Where(index => Vector3.Dot(stitches[index].Normal, cameraForward) < 0).ToList();
        //List<int> indicesInViewAngle = indicesToCheck.Where(index => StitchAngle(stitches[index].Normal) >-90 && StitchAngle(stitches[index].Normal) <90).ToList();
        //Debug.Log(indicesInViewAngle.Count());
        List<int> indicesInRange = indicesInViewDir.Where(index => ScreenDistance(screenPositions[index], Input.mousePosition) <= _cellSize).ToList();
        if (indicesInRange.Count!=0)Debug.Log(StitchAngle(stitches[indicesInRange[0]].Normal));
        TrySetHoveredStitches(indicesInRange,screenPositions,stitches);
        
        
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

    private static int GetClosestScreenPositionIndexInRadius(IList<int> positionIndices, IList<Vector3> screenPositions, Vector3 targetPosition, float checkRadius)
    {
        int closestIndex = -1;
        float smallestDepth = float.MaxValue;
        if (positionIndices.Count == 0)
        {
            return closestIndex;
        }

        for (int i = 0; i < positionIndices.Count; i++)
        {
            int index = positionIndices[i];
            Vector3 screenPos = screenPositions[index];
            var distance = ScreenDistance(screenPos, targetPosition);
            if (distance > checkRadius)
            {
                continue;
            }

            if (screenPos.z < smallestDepth)
            {
                smallestDepth = screenPos.z;
                closestIndex = index;
            }
        }
        return closestIndex;
    }

    private T GetClosest<T>(IList<T> items, Func<T, Vector3> positionGetter, out int closestIndex)
    {
        closestIndex = -1;
        if (items.Count == 0)
        {
            return default;
        }
        float closestDistance = float.MaxValue;
        T closest = default;
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var distance = DistanceToMousePixels(positionGetter(item));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = item;
                closestIndex = i;
            }
        }
        
        return closest;
    }

    private VerletNode CheckForAnchoredNode()
    {
        var anchoredNodes = GameManager.Instance.Project.anchors.AnchoredNodes();
        if (anchoredNodes.Length == 0) return null;
        var nodesInsideRadius = FilterByRadius(anchoredNodes, node => node.Position, _cellSize);
        if (nodesInsideRadius.Count == 0) return null;

        return GetClosest(nodesInsideRadius, node => node.Position, out int index);
    }
    
    private void TrySetHoveredStitches(IEnumerable<int> stitchIndexCandidates, IList<Vector3> screenPositions, IList<Stitch> stitches)
    {
        if (SelectedNode != null) return;
        foreach (var (key, value) in GameManager.Instance.Project.anchors.GetAnchors())
        {
            value.SetHighlight(0f);
        }
        Vector2 mousePos = NormalizePixelCoords(Input.mousePosition);
        float closestDistance = float.MaxValue; // Track the closest stitch

        IndicesInRadius = new List<int>(stitchIndexCandidates);

        Stitch[] stitchesInRadius = IndicesInRadius.Select(index => stitches[index]).ToArray();
        HoveredStitch = GetClosest(stitchesInRadius, stitch => stitch.Position, out int hoveredStitchIndex);
        
        if (HoveredStitch == null) return;

        if (MouseRadius == 0)
        {
            int hoveredStitch = IndicesInRadius[hoveredStitchIndex];
            HoverStitchStatus[hoveredStitch] = true;
            IndicesInRadius.Clear();
            IndicesInRadius.Add(hoveredStitch);
        }
        else
        {
            foreach (var i in IndicesInRadius)
            {
                HoverStitchStatus[i] = true;
            }
        }
        
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
    { //converts worldspace to screenspace and compares to mousepos
        Vector2 point = _cam.WorldToScreenPoint(worldPos);
        return (point - (Vector2)Input.mousePosition).magnitude;
    }

    private static float ScreenDistance(Vector3 screenPos1, Vector3 screenPos2)
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

    public float StitchAngle(Vector3 normal)
    {
        var transform = _cam.transform;
        return Vector3.SignedAngle(normal, transform.forward, transform.up);
    }
}

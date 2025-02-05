using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class Panel
{
    public List<Stitch> Stitches { get; private set; } = new List<Stitch>(); //set in connector right after initializing
    public List<VerletNode> Nodes { get; private set; }
    public List<VerletNode> AnchoredNodes { get; private set; }
    public bool IsCircular { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string Name { get; private set; }

    public void CreatePanel(PanelConfig panelConfig)
    {
        Width = panelConfig.Dimensions.x +1; //size in nodes, not stitches
        Height = panelConfig.Dimensions.y +1;
        IsCircular = panelConfig.IsCircular;
        Name = panelConfig.Name;
        AnchoredNodes = new List<VerletNode>();

        if (!panelConfig.Tube) Nodes = GenerateNodes(panelConfig);
        else Nodes = GenerateNodesTube(panelConfig);
        Connector.ConnectNodes(this, panelConfig.Gauge);
        //make array of nodes and send to connector
        Connector.ConnectStitches(Stitches);
    }
    
    private List<VerletNode> GenerateNodes(PanelConfig panelConfig)
    {
        var nodes = new List<VerletNode>();
        
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Vector2Int coordinate;
                    if (panelConfig.Reverse) coordinate = new Vector2Int(Width - x, y);
                    else coordinate = new Vector2Int(x, y);
                    VerletNode node = new VerletNode(FlatPanelPosition(panelConfig.StartPos, panelConfig.Gauge, coordinate, panelConfig.StartPos.z)); // Replace the line above with this 
                    nodes.Add(node);
                    node.SetParentPanel(this);
                    node.SetSize(10f/panelConfig.Gauge.x, 10f/panelConfig.Gauge.y);
                }
            }
        
        return nodes;
    }

    private List<VerletNode> GenerateNodesTube(PanelConfig panelConfig)
    {
        var nodes = new List<VerletNode>();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector3 pos = TubePanelPosition(Vector3.zero, panelConfig.Gauge, new Vector2Int(x, y),
                    panelConfig.Dimensions.x + 1);
                var rotation = 0;
                if (panelConfig.Rotation == PanelConfig.rotationEnum.LEFT)
                {
                    rotation = -90;
                }

                if (panelConfig.Rotation == PanelConfig.rotationEnum.RIGHT)
                {
                    rotation = 90;
                }
                
                pos = Quaternion.Euler(0, 0, rotation) * pos;
                pos += panelConfig.StartPos;
                VerletNode node = new VerletNode(pos); // Replace the line above with this 
                nodes.Add(node);
                node.SetParentPanel(this);
                node.SetSize(10f/panelConfig.Gauge.x, 10f/panelConfig.Gauge.y);
            }
        }

        return nodes;
    }

    public static Vector3 FlatPanelPosition(Vector3 lowerLeft, Vector2Int gauge, Vector2Int coordinate, float zPos)
    {
        float comp1 = lowerLeft.x + 10f / gauge.x * coordinate.x;
        float comp2 = lowerLeft.y + 10f / gauge.y * coordinate.y;
        return new Vector3(comp1, comp2, zPos);
    }
    
    public static Vector3 TubePanelPosition(Vector3 basePoint, Vector2Int gauge, Vector2Int coordinate, int tubeSegments)
    {
        float vertical = 10f / gauge.y * coordinate.y;
        float angle = (coordinate.x / (float)tubeSegments) * Mathf.PI * 2;
        float circumference = tubeSegments * (10f / gauge.x);
        float radius = circumference / (2 * Mathf.PI);
        float horizontal1 = Mathf.Cos(angle) * radius;
        float horizontal2 = Mathf.Sin(angle) * radius;
        return new Vector3(horizontal1, vertical, horizontal2) + basePoint;
    }
    
    private void SelectPanel()
    {
        
    }

    public void UpdateStitchPosition()
    {
        foreach (var s in Stitches)
        {
            s.UpdatePosition();
        }
    }
}

public class PanelConfig
{
    public Vector2Int Dimensions { get; }
    public bool IsCircular { get; }
    public Vector2Int Gauge { get; }
    public string Name { get; }
    public Vector3 StartPos { get; }
    public bool Tube { get; }
    public bool Reverse { get; }
    public rotationEnum Rotation;
    public enum rotationEnum
    {
        DEFAULT,
        LEFT,
        RIGHT
    }

    public PanelConfig(string name, Vector2Int dimensions, bool isCircular, Vector2Int gauge, Vector3 startPos, bool tube = false, bool reverse = false, rotationEnum rotation = rotationEnum.DEFAULT)
    {
        Name = name;
        Dimensions = dimensions;
        IsCircular = isCircular;
        Gauge = gauge;
        StartPos = startPos;
        Tube = tube;
        Reverse = reverse;
        Rotation = rotation;

    }
}


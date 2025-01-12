using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class Panel
{
    public List<Stitch> Stitches { get; private set; } //set in connector right after initializing
    public List<VerletNode> Nodes { get; private set; }
    public bool IsCircular { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int HorizontalGauge { get; private set; }
    public int VerticalGauge { get; private set; }

    public void CreatePanel(Vector2Int dimensions, bool myIsCircular, Vector2Int myGauge)
    {
        Width = dimensions.x +1; //size in nodes, not stitches
        Height = dimensions.y +1;
        HorizontalGauge = myGauge.x;
        VerticalGauge = myGauge.y;
        IsCircular = myIsCircular;
        
        Nodes = GenerateNodes(new Vector2Int(Width,Height));
        NodeConnector.ConnectNodes(this);
        //make array of nodes and send to connector
    }
    
    private List<VerletNode> GenerateNodes(Vector2Int myDimensions)
    {
        var nodes = new List<VerletNode>();
        for (int y = 0; y < myDimensions.y; y++)
        {
            for (int x = 0; x < myDimensions.x; x++)
            {
                VerletNode node = new VerletNode(new Vector2(10f/HorizontalGauge * x,10f/VerticalGauge * y));
                nodes.Add(node);
            }
        }

        return nodes;
    }
    
    private void RemovePanel()
    {
        
    }

    private void SelectPanel()
    {
        
    }
}


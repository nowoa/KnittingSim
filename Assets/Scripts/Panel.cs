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

    public void CreatePanel(Vector2Int dimensions, bool myIsCircular, Vector2Int myGauge, string myName)
    {
        Width = dimensions.x +1; //size in nodes, not stitches
        Height = dimensions.y +1;
        IsCircular = myIsCircular;
        Name = myName;
        AnchoredNodes = new List<VerletNode>();
        
        Nodes = GenerateNodes(new Vector2Int(Width,Height), myGauge);
        Connector.ConnectNodes(this, myGauge);
        //make array of nodes and send to connector
        Connector.ConnectStitches(Stitches);
    }
    
    private List<VerletNode> GenerateNodes(Vector2Int myDimensions, Vector2Int myGauge)
    {
        var nodes = new List<VerletNode>();
        for (int y = 0; y < myDimensions.y; y++)
        {
            for (int x = 0; x < myDimensions.x; x++)
            {
                var z = Random.value * 0.01f;
                VerletNode node = new VerletNode(new Vector3(10f/myGauge.x * x,10f/myGauge.y * y, z));
                nodes.Add(node);
                node.SetParentPanel(this);
                node.SetSize(10f/myGauge.x, 10f/myGauge.y);
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

    public void UpdateStitchPosition()
    {
        foreach (var s in Stitches)
        {
            s.UpdatePosition();
        }
    }
}


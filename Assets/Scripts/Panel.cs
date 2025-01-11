using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class Panel
{
    public List<Stitch> Stitches; //set in connector right after initializing
    public List<VerletNode> Nodes { get; private set; }
    public bool IsCircular { get; private set; }
    private int _width;
    private int _height;
    public int HorizontalGauge { get; private set; }
    public int VerticalGauge { get; private set; }

    private void GenerateNodes()
    {
        //create nodes based on panel information
    }
    
    private void RemovePanel()
    {
        
    }

    private void CreatePanel(int myWidth, int myHeight, bool myIsCircular)
    {
        GenerateNodes();
        //make array of nodes and send to connector
    }

    private void SelectPanel()
    {
        
    }
}


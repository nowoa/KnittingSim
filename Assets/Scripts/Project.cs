using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Verlet;

public class Project
{
    #region Variables

    private Dictionary<string, Panel> _panels = new();
    private FabricMesh _fabricMesh;
    private List<VerletNode> _nodes = new ();
    public VerletSimulator Simulator;

    #endregion

    public Project()
    {
        var gameObject = new GameObject();
        gameObject.AddComponent<FabricMesh>();
    }

    public void AddPanel(string myName,Vector2Int myDimensions, bool myIsCircular, Vector2Int myGauge)
    {
        _panels.Add(myName,new Panel());
        _panels[myName].CreatePanel(myDimensions,myIsCircular, myGauge);
        _nodes.AddRange(_panels[myName].Nodes);
        Simulator = new VerletSimulator(_nodes);
    }

    public void UpdatePanels()
    {
        foreach (var pair in _panels)
        {
            pair.Value.UpdateStitchPosition();
        }
    }
}


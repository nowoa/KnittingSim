using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using Verlet;

public class Project
{
    #region Variables

    private Dictionary<string, Panel> _panels = new();
    private FabricMesh _fabricMesh;
    public List<VerletNode> Nodes { get; private set; } = new();
    public VerletSimulator Simulator;

    #endregion

    public Project()
    {
        var gameObject = GameObject.FindWithTag("GameController");
        _fabricMesh = gameObject.AddComponent<FabricMesh>();
        Simulator = new VerletSimulator(Nodes);
    }

    public void AddPanel(string myName,Vector2Int myDimensions, bool myIsCircular, Vector2Int myGauge)
    {
        _panels.Add(myName,new Panel());
        _panels[myName].CreatePanel(myDimensions,myIsCircular, myGauge,myName);
        Nodes.AddRange(_panels[myName].Nodes);
        _fabricMesh.UpdateMesh();
    }

    public void UpdatePanels()
    {
        foreach (var pair in _panels)
        {
            pair.Value.UpdateStitchPosition();
        }
    }

    public void UpdateMesh()
    {
        _fabricMesh.UpdatePositions();
    }

    public List<Panel> GetPanels()
    {
        return _panels.Values.ToList();
    }

    public void AnchorNodes()
    {
        foreach (var p in _panels.Values)
        {
            p.SetAnchoredPosition();
        }
    }
}


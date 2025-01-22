using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using Verlet;

public class Project
{
    #region Variables

    private Dictionary<string, Panel> _panels = new();
    public FabricMesh FabricMesh;
    public List<VerletNode> Nodes { get; private set; } = new();
    public VerletSimulator Simulator;
    

    #endregion

    public Project()
    {
        var gameObject = GameObject.FindWithTag("GameController");
        FabricMesh = gameObject.AddComponent<FabricMesh>();
        Simulator = new VerletSimulator(Nodes);
        GameManager.Instance.EventManager.OnRegenerateMesh += RegenerateFabricMesh;
    }

    public void AddPanel(string myName,Vector2Int myDimensions, bool myIsCircular, Vector2Int myGauge)
    {
        _panels.Add(myName,new Panel());
        _panels[myName].CreatePanel(myDimensions,myIsCircular, myGauge,myName);
        Nodes.AddRange(_panels[myName].Nodes);
        GameManager.Instance.EventManager.InvokeRegenerateMesh();
    }

    public void FixedUpdate(int mySimIterations, float dt)
    {
        AnchorNodes();
        Simulator.Simulate(mySimIterations,dt);
        UpdatePanelPosition();
        CalculateNormals();
        UpdateMeshPosition();
    }

    public void RegenerateFabricMesh()
    {
        FabricMesh.RegenerateMesh(GetPanels().SelectMany(item => item.Stitches).ToList());
        UpdateMeshPosition();
    }

    public void UpdatePanelPosition()
    {
        foreach (var pair in _panels)
        {
            pair.Value.UpdateStitchPosition();
        }
    }

    public void CalculateNormals()
    {
        foreach (var stitch in GetPanels().SelectMany(item => item.Stitches))
        {
            stitch.UpdateNormal();
        }

        foreach (var node in GetPanels().SelectMany(item => item.Nodes))
        {
            node.UpdateNormal();
        }
    }

    public void UpdateMeshPosition()
    {
        var positions = new List<Vector3>();
        var normals = new List<Vector3>();
        foreach (var s in GetPanels().SelectMany(item => item.Stitches))
        {
            var corners = s.GetCorners();
            positions.AddRange(corners.Select(item => item.Position));
            normals.AddRange(corners.Select(item=>item.Normal));
        }
        //update positions every frame
        FabricMesh.UpdatePositions(positions.ToArray(), normals.ToArray());
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


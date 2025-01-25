using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Verlet;

public class Project
{
    #region Variables

    private Dictionary<string, Panel> _panels = new();
    public FabricMesh FabricMesh;
    public Dictionary<Vector3Int, List<int>> SpatialHashGrid = new();
    public VerletNode[] Nodes { get; private set; } = Array.Empty<VerletNode>();
    public Stitch[] Stitches { get; private set; } = Array.Empty<Stitch>();
    public VerletSimulator Simulator;
    public bool Collision = false;
    

    #endregion

    public Project()
    {
        var gameObject = GameObject.FindWithTag("GameController");
        FabricMesh = gameObject.AddComponent<FabricMesh>();
        Simulator = new VerletSimulator(Nodes);
        GameManager.Instance.EventManager.OnRegenerateMesh += UpdateFabricStructure;
    }

    public void AddPanel(string myName,Vector2Int myDimensions, bool myIsCircular, Vector2Int myGauge)
    {
        _panels.Add(myName,new Panel());
        _panels[myName].CreatePanel(myDimensions,myIsCircular, myGauge,myName);
        UpdateGlobalNodesAndStitches();
        GameManager.Instance.EventManager.InvokeStructureUpdate();
    }

    public void FixedUpdate(int mySimIterations, float dt)
    {
        if (_panels.Count == 0)
        {
            return;
        }
        AnchorNodes();

        float partitioningCellSize = 0.5f;

        using (new ProfileSample("Construct Spatial Hash Grid"))
            SpatialHashGrid = DefaultNamespace.SpatialHashGrid.PartitionIndex(Nodes, node => node.Position, partitioningCellSize);
        
        using (new ProfileSample("Solve Self Collision"))
            if(Collision) SelfCollision.Solve(Nodes, SpatialHashGrid, partitioningCellSize);
        using (new ProfileSample("Solve Self Collision"))
            if(Collision) SelfCollision.Solve(Nodes, SpatialHashGrid, partitioningCellSize);
        using (new ProfileSample("Verlet Simulation"))
            Simulator.Simulate(mySimIterations,dt);


        using (new ProfileSample("Mesh Update"))
        {
            UpdatePanelPosition();
            CalculateNormals();
            UpdateMeshPosition();
        }
    }

    public void UpdateFabricStructure()
    {//updates the global node and stitch lists, then updates the mesh
        UpdateGlobalNodesAndStitches();
        foreach (var n in Nodes)
        {
           n.GetDirectNeighbors(); 
        }
        FabricMesh.RegenerateMesh(Stitches);
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
        foreach (var stitch in Stitches)
        {
            stitch.UpdateNormal();
        }

        foreach (var node in Nodes)
        {
            node.UpdateNormal();
        }
    }

    public void UpdateMeshPosition()
    {
        var positions = new List<Vector3>();
        var normals = new List<Vector3>();
        foreach (var s in Stitches)
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

    public void UpdateGlobalNodesAndStitches()
    {
        Nodes = GetPanels().SelectMany(item => item.Nodes).ToArray();
        Stitches = GetPanels().SelectMany(item => item.Stitches).ToArray();
        Simulator = new VerletSimulator(Nodes);
    }
}


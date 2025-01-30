using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

public class Project
{
    #region Variables

    private Dictionary<string, Panel> _panels = new();
    public FabricMesh FabricMesh;
    public Dictionary<Vector3Int, List<int>> HashGridWorld = new();
    public Vector3[] StitchScreenPositions;
    public Dictionary<Vector2Int, List<Stitch>> ScreenHashGrid = new();
    public Dictionary<Vector2Int, List<int>> HashGridScreen = new();
    public VerletNode[] Nodes { get; private set; } = Array.Empty<VerletNode>();
    public Stitch[] Stitches { get; private set; } = Array.Empty<Stitch>();
    public Anchors anchors = new Anchors();
    public VerletSimulator Simulator;
    public bool Collision = true;
    private float _partitioningCellSize;
    private ToolBoxUI _toolBoxUI;

    #endregion

    public Project()
    {
        FabricMesh = GameManager.Instance.gameObject.AddComponent<FabricMesh>();
        _toolBoxUI = ToolBoxUI.Instance;
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
        _toolBoxUI.UpdateUI();
        if (_panels.Count == 0)
        {
            return;
        }

        anchors.UpdateNodePositions();

        Simulator.Simulate(mySimIterations,dt);
        
        HashGridWorld = SpatialHashGrid.PartitionIndex(Nodes, node => node.Position, _partitioningCellSize);
        StitchScreenPositions = Stitches.Select(stitch => GameManager.Instance.Camera.WorldToScreenPoint(stitch.Position)).ToArray();
        HashGridScreen = SpatialHashGrid.PartitionScreen(StitchScreenPositions, pos => pos, GameManager.Instance.Hover.MouseRadius);
        
        if(Collision) SelfCollision.Solve(Nodes, HashGridWorld, _partitioningCellSize);
        
        UpdatePanelPosition();
        CalculateNormals();
        anchors.UpdatePinPositions();
        UpdateMeshPosition();
    }

    public void UpdateFabricStructure()
    {//updates the global node and stitch lists, then updates the mesh
        UpdateGlobalNodesAndStitches();
        foreach (var n in Nodes)
        {
           n.GetDirectNeighbors(); 
        }

        _partitioningCellSize = Nodes[0].CollisionRadius * 1f;
        FabricMesh.RegenerateMesh(Stitches);
        FabricMesh.SetVertexColors(Stitches);
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
        var detailUVs = new List<Vector2>();
        
        bool[] hoveredStitches = GameManager.Instance.Hover.HoverStitchStatus;
        bool hasHoveredStitches = hoveredStitches is not null;
        
        for(int i = 0; i < Stitches.Length; i++)
        {
            Stitch stitch = Stitches[i];
            var corners = stitch.GetCorners();
            positions.AddRange(corners.Select(item => item.Position));
            normals.AddRange(corners.Select(item=>item.Normal));
            detailUVs.AddRange(corners.Select(item=> new Vector2(hasHoveredStitches && hoveredStitches[i]? 1: 0, 0)));
        }
        //update positions every frame
        FabricMesh.UpdatePositions(positions.ToArray(), normals.ToArray(), detailUVs);
    }

    public List<Panel> GetPanels()
    {
        return _panels.Values.ToList();
    }

    public void UpdateGlobalNodesAndStitches()
    {
        Nodes = GetPanels().SelectMany(item => item.Nodes).ToArray();
        Stitches = GetPanels().SelectMany(item => item.Stitches).ToArray();
        Simulator = new VerletSimulator(Nodes);
    }
}


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
    public Vector3[] NodeScreenPositions;
    public Dictionary<Vector2Int, List<Stitch>> ScreenHashGrid = new();
    public Dictionary<Vector2Int, List<int>> HashGridScreen = new();
    public float MinimumCellSize { get; private set; }
    public VerletNode[] Nodes { get; private set; } = Array.Empty<VerletNode>();
    public Stitch[] Stitches { get; private set; } = Array.Empty<Stitch>();
    public Anchors anchors = new Anchors();
    public VerletSimulator Simulator;
    public bool Collision = true;
    private float _partitioningCellSize;
    private ToolBoxUI _toolBoxUI;
    public bool MeshUpdated;

    #endregion

    public Project()
    {
        FabricMesh = GameManager.Instance.gameObject.AddComponent<FabricMesh>();
        _toolBoxUI = ToolBoxUI.Instance;
        Simulator = new VerletSimulator(Nodes);
        /*GameManager.Instance.EventManager.OnRegenerateMesh += UpdateFabricStructure;*/
    }

    public void AddPanel(string myName,Vector2Int myDimensions, bool myIsCircular, Vector2Int myGauge, Vector3 startPos)
    {
        _panels.Add(myName,new Panel());
        _panels[myName].CreatePanel(myDimensions,myIsCircular, myGauge,myName, startPos);
        UpdateGlobalNodesAndStitches();
        GameManager.Instance.EventManager.InvokeStructureUpdate();
    }

    public void FixedUpdate()
    {
        
    }

    public void FixedUpdatePreHover(int mySimIterations, float dt)
    {
        if (_panels.Count == 0)
        {
            return;
        }

        if (MeshUpdated)
        {
            UpdateFabricStructure();
            MeshUpdated = false;
        }

        anchors.UpdateNodePositions();

        Simulator.Simulate(mySimIterations,dt);
        
        HashGridWorld = SpatialHashGrid.PartitionIndex(Nodes, node => node.Position, _partitioningCellSize);
        StitchScreenPositions = Stitches.Select(stitch => GameManager.Instance.Camera.WorldToScreenPoint(stitch.Position)).ToArray();

        
        using (new ProfileSample("minimum cellsize computation"))
        {
            
            NodeScreenPositions = new Vector3[Nodes.Length];
            foreach (var n in Nodes)
            {
                NodeScreenPositions[n.id] = GameManager.Instance.Camera.WorldToScreenPoint(n.Position);
            }

            MinimumCellSize = ComputeMinimumCellSize();
        }
        
        var mouseRadius = GameManager.Instance.Hover.MouseRadius;
        
        using (new ProfileSample("create hashgrid"))
            HashGridScreen = SpatialHashGrid.PartitionScreen(StitchScreenPositions, pos => pos, Mathf.Max(MinimumCellSize, mouseRadius));
        
        if(Collision) SelfCollision.Solve(Nodes, HashGridWorld, _partitioningCellSize);
        
        UpdatePanelPosition();
        CalculateNormals();
        anchors.UpdatePinPositions();
    }

    public void FixedUpdatePostHover()
    {
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

    public Panel GetPanelByName(string name)
    {
        return _panels[name];
    }

    public void FrogAll()
    {
        _panels.Clear();
        anchors.RemoveAllAnchors();
        Nodes = Array.Empty<VerletNode>();
        Stitches = Array.Empty<Stitch>();
        Connector.ResetIDs();
        FabricMesh.DestroyMesh();
    }

    public void UpdateGlobalNodesAndStitches()
    {
        Nodes = GetPanels().SelectMany(item => item.Nodes).ToArray();
        Stitches = GetPanels().SelectMany(item => item.Stitches).ToArray();
        Simulator = new VerletSimulator(Nodes);
    }

    private float ComputeMinimumCellSize()
    {
        var largestDistance = 0f;
        foreach (var s in Stitches)
        {
            var distance1 = (NodeScreenPositions[s.Corners[0].id] - NodeScreenPositions[s.Corners[2].id]).sqrMagnitude;
            var distance2 = (NodeScreenPositions[s.Corners[1].id] - NodeScreenPositions[s.Corners[3].id]).sqrMagnitude;
            var result = distance1 > distance2 ? distance1 : distance2;
            largestDistance = result > largestDistance ? result : largestDistance;
        }

        largestDistance = Mathf.Sqrt(largestDistance);
        return largestDistance;
    }
}


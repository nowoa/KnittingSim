using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;
using Random = UnityEngine.Random;
using Vector2Int = UnityEngine.Vector2Int;

public class FabricManager
{
    private GarmentGenerator _parent;
    private Camera _camera;
    private MouseDragger _mouseDragger = MouseDragger.Instance;
    private readonly Dictionary<string, PanelInfo> _panelDictionary = new();
    private readonly Dictionary<string, List<VerletNode>> _seamDictionary = new();
    private List<StitchInfo> _stitchInfos;
    private VerletSimulator _sim;
    private VerletSimulator _simConnected;
    public static int NodeCount;
    private List<VerletNode> _anchoredNodes = new();
    private string _panelName;
    private bool _isCircular;
    private VerletNode _closestNode;
    public static event Action UpdateSimulation;
    public static List<VerletNode> AllNodes = new();
    public static List<StitchInfo> AllStitches = new();

    public static void InvokeUpdateSimulation()
    {
        if (UpdateSimulation != null)
        {
            UpdateSimulation.Invoke();
        }
        else{Debug.Log("no panel created yet!");}
    }
    
    public FabricManager(GarmentGenerator parent)
    {
        _parent = parent;
        ClearPreviousData();
    }

    private void ClearPreviousData()
    {
        AllStitches.Clear();
        AllNodes.Clear();
    }

    public void MakePanel(string myPanelName, int myPanelWidth, int myPanelHeight, bool myIsCircular)
    {
        List<VerletNode> nodes = InitNodes(new Vector2Int(myPanelWidth+1,myPanelHeight+1));
        var thisPanelInfo = new PanelInfo(nodes, myPanelWidth+1, myPanelHeight+1, myIsCircular);
        _panelDictionary[myPanelName] = thisPanelInfo;
        Connect(myPanelName);
        _sim = new VerletSimulator(GetAllNodes());
        UpdateSimulation += OnSimulationUpdate;
        foreach (var s in AllStitches)
        {
            s.SetSize(new Vector2(_parent.StitchTemplate.width, _parent.StitchTemplate.height));
        }
        Stretch();
    }

    public PanelInfo GetPanelInfo(string key)
    {
        return _panelDictionary[key];
    }

    public List<VerletNode> GetSeam(string key)
    {
        return _seamDictionary[key];
    }

    public void ConnectSeams(string key1, string key2)
    {
        NodeConnector.ConnectSeams(_seamDictionary[key1], _seamDictionary[key2]);
    }

    private List<VerletNode> InitNodes(Vector2Int myDimensions)
    {
        var nodes = new List<VerletNode>();
        for (int y = 0; y < myDimensions.y; y++)
        {
            for (int x = 0; x < myDimensions.x; x++)
            {
                VerletNode node = new VerletNode(Random.insideUnitSphere);
                nodes.Add(node);
            }
        }

        return nodes;
    }

    private void Connect(string myPanelName)
    {
        var panel = _panelDictionary[myPanelName];
        NodeConnector.ConnectNodes(panel.Nodes, panel.Width, panel.IsCircular, _parent.StitchTemplate);
    }

    public void CreateSeam(string myPanelName, string mySeamKey, Vector2Int myStartingPoint, Vector2Int myEndPoint,
        int myLength, bool useCeil = false)
    {
        _seamDictionary.TryAdd(mySeamKey, new List<VerletNode>());
        float lerpStep = 1 / (float)myLength;

        for (int i = 1; i <= myLength; i++)
        {
            float lerpedX = Mathf.Lerp(myStartingPoint.x, myEndPoint.x, lerpStep * i);
            float lerpedY = Mathf.Lerp(myStartingPoint.y, myEndPoint.y, lerpStep * i);

            int x = useCeil ? (int)Mathf.Ceil(lerpedX) : (int)Mathf.Floor(lerpedX);
            int y = useCeil ? (int)Mathf.Ceil(lerpedY) : (int)Mathf.Floor(lerpedY);

            _seamDictionary[mySeamKey].Add(_panelDictionary[myPanelName].GetNodeAt(x, y));
        }
    }

    public void AnchorNodeCoordinate(string myPanelName, Vector2Int myCoordinate)
    {
        _panelDictionary[myPanelName].GetNodeAt(myCoordinate.x, myCoordinate.y).IsAnchored = true;
    }

    public void AnchorNodeIndex(string myPanelName, int index)
    {
        _panelDictionary[myPanelName].Nodes[index].IsAnchored = true;
    }


    //get all nodes
    public List<VerletNode> GetAllNodes()
    {
        List<VerletNode> nodesToSimulate = new List<VerletNode>();
        foreach (PanelInfo myPanelInfo in _panelDictionary.Values)
        {
            nodesToSimulate.AddRange(myPanelInfo.Nodes);
        }

        NodeCount = nodesToSimulate.Count;
        AllNodes = nodesToSimulate;
        return nodesToSimulate;
    }

    public IEnumerable<VerletNode> IterateAllNodes()
    {
        foreach (PanelInfo info in _panelDictionary.Values)
        {
            foreach (var n in info.Nodes) yield return n;
        }
    }

    public void OnSimulationUpdate()
    {
        _sim = new VerletSimulator(AllNodes);
    }
    

    private void GetConnectedNodes()
    {
        if (_mouseDragger.SelectedChildIndex == -1) return;
        var centralNode = AllNodes[_mouseDragger.SelectedChildIndex];
        var connectedNodes = new HashSet<VerletNode>();
        AddConnectedNodes(centralNode, connectedNodes, 3);
        _simConnected = new VerletSimulator(connectedNodes.ToList());
    }

    private void AddConnectedNodes(VerletNode node, HashSet<VerletNode> connectedNodes, int depth)
    {
        if (depth <= 0) return;
        foreach (var edge in node.Connection)
        {
            var otherNode = edge.Other(node);
            if (connectedNodes.Add(otherNode))
            {
                AddConnectedNodes(otherNode, connectedNodes, depth - 1);
            }
        }
    }

    public void AnchorNode(VerletNode node, Vector3 myPosition)
    {
        node.IsAnchored = true;
        node.AnchoredPos = myPosition;
    }
    
    public void Stretch()
    {
        foreach (var panel in _panelDictionary.Values)
        {
            panel.Nodes[0].IsAnchored = true;
            panel.Nodes[0].AnchoredPos = new Vector3(-0.5f*((panel.Width) * _parent.StitchTemplate.width * 1.5f), -0.5f*((panel.Height) * _parent.StitchTemplate.height * 1.5f), 0);
            
            panel.Nodes[panel.Width-1].IsAnchored = true;
            panel.Nodes[panel.Width-1].AnchoredPos =
                new Vector3(0.5f*((panel.Width) * _parent.StitchTemplate.width * 1.5f), -0.5f*((panel.Height) * _parent.StitchTemplate.height * 1.5f), 0);
            
            panel.Nodes[^panel.Width].IsAnchored = true;
            panel.Nodes[^panel.Width].AnchoredPos =
                new Vector3(-0.5f*((panel.Width) * _parent.StitchTemplate.width * 1.5f), 0.5f*((panel.Height) * _parent.StitchTemplate.height * 1.5f), 0);
            
            panel.Nodes.Last().IsAnchored = true;
            panel.Nodes.Last().AnchoredPos = new Vector3(0.5f*((panel.Width) * _parent.StitchTemplate.width * 1.5f),
                0.5f*((panel.Height) * _parent.StitchTemplate.height * 1.5f), 0);
            
        }
    }

    public void FixedUpdate()
    {
        if (_mouseDragger.SelectedChildIndex != -1)
        {
            AllNodes[_mouseDragger.SelectedChildIndex].Position = _mouseDragger.GetTargetPos();
            AllNodes[_mouseDragger.SelectedChildIndex].AnchoredPos = _mouseDragger.GetTargetPos();
        }

        /*if (_simConnected != null)
        {
            _simConnected.Simulate(3, Time.fixedDeltaTime);
        }*/

        if (_sim != null)
        {
            foreach (var node in _sim.Nodes)
            {
                if (node.IsAnchored)
                {
                    node.Position = node.AnchoredPos;
                }
            }

            _sim.Simulate(2, Time.fixedDeltaTime);
        }
        
        //update stitch position
        foreach (var stitch in AllStitches)
        {
            if (stitch.corners.Any(item => item != null))
            {
                stitch.SetPosition(Calculation.GetStitchPosition(stitch.corners));
            }
        }
        
    }

    public void DrawGizmos()
    {
        if (_sim != null)
        {
            _sim.DrawGizmos(Color.white);
        }

        if (_simConnected != null)
        {
            _simConnected.DrawGizmos(Color.magenta);
        }

        if (_mouseDragger.HoveredChildIndex != -1)
        {
            Gizmos.DrawSphere(AllNodes[_mouseDragger.HoveredChildIndex].Position, 0.1f);
        }
        
        if (_mouseDragger.HoveredStitchIndex != -1 && !AllStitches[_mouseDragger.HoveredStitchIndex].isInactive)
        {
            Gizmos.color=Color.red;
            Gizmos.DrawSphere(AllStitches[_mouseDragger.HoveredStitchIndex].corners[0].Position, 0.05f);
            Gizmos.color=Color.yellow;
            Gizmos.DrawSphere(AllStitches[_mouseDragger.HoveredStitchIndex].corners[1].Position, 0.05f);
            Gizmos.color=Color.green;
            Gizmos.DrawSphere(AllStitches[_mouseDragger.HoveredStitchIndex].corners[2].Position, 0.05f);
            Gizmos.color=Color.blue;
            Gizmos.DrawSphere(AllStitches[_mouseDragger.HoveredStitchIndex].corners[3].Position, 0.05f);
        }
        if (_sim != null)
        {
            foreach (var node in _sim.Nodes)
            {
                if (node.IsAnchored)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(node.Position,0.1f);
                }
            }
        }
        
    }

    public void RenderNodes(Material material, Mesh mesh)
    {
        var rparams = new RenderParams(material);
        List<Matrix4x4> renderMatrices = new();
        foreach (var node in AllNodes)
        {
            Matrix4x4 mat = Matrix4x4.TRS(node.Position, Quaternion.identity, Vector3.one * 0.1f);
            renderMatrices.Add(mat);
        }
        Graphics.RenderMeshInstanced(rparams, mesh, 0, renderMatrices);
    }
}
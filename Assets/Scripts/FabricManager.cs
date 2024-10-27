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

    public FabricManager(GarmentGenerator parent)
    {
        _parent = parent;
    }
    
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

    public void MakePanel(string myPanelName, int myPanelWidth, int myPanelHeight, bool myIsCircular)
    {
        _stitchInfos = new List<StitchInfo>(GridMaker.MakePanel(new Vector2Int(myPanelWidth, myPanelHeight)));
        List<VerletNode> nodes = InitNodes(_stitchInfos);
        var thisPanelInfo = new PanelInfo(nodes, myPanelWidth, myPanelHeight, myIsCircular);
        _panelDictionary[myPanelName] = thisPanelInfo;
        Connect(myPanelName);
        _sim = new VerletSimulator(GetAllNodes());
        AllNodes = GetAllNodes();
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

    List<VerletNode> InitNodes(List<StitchInfo> myStitchInfos)
    {
        var nodes = new List<VerletNode>();
        for (int i = 0; i < myStitchInfos.Count; i++)
        {
            var newNode = new VerletNode(Random.insideUnitSphere);
            nodes.Add(newNode);
        }

        return nodes;
    }

    private void Connect(string myPanelName)
    {
        var panel = _panelDictionary[myPanelName];
        
        NodeConnector.ConnectNodes(panel.Nodes, panel.Width, panel.IsCircular, _parent.StitchTemplate);
    }

    public void CreateSeam(string myPanelName, string mySeamKey, Vector2Int myStartingPoint, Vector2Int myEndPoint,
        int myLength)
    {
        _seamDictionary.TryAdd(mySeamKey, new List<VerletNode>());

        float lerpStep = 1 / (float)myLength;
        for (int i = 1; i <= myLength; i++)
        {
            int x = (int)Mathf.Floor(Mathf.Lerp(myStartingPoint.x, myEndPoint.x, lerpStep * i));
            int y = (int)Mathf.Floor(Mathf.Lerp(myStartingPoint.y, myEndPoint.y, lerpStep * i));

            _seamDictionary[mySeamKey].Add(_panelDictionary[myPanelName].GetNodeAt(x, y));
        }
    }

    public void CreateSeamReverse(string myPanelName, string mySeamKey, Vector2Int myStartingPoint,
        Vector2Int myEndPoint, int myLength)
    {
        _seamDictionary.TryAdd(mySeamKey, new List<VerletNode>());

        float lerpStep = 1 / (float)myLength;
        for (int i = 1; i <= myLength; i++)
        {
            int x = (int)Mathf.Ceil(Mathf.Lerp(myStartingPoint.x, myEndPoint.x, lerpStep * i));
            int y = (int)Mathf.Ceil(Mathf.Lerp(myStartingPoint.y, myEndPoint.y, lerpStep * i));

            _seamDictionary[mySeamKey].Add(_panelDictionary[myPanelName].GetNodeAt(x, y));
        }
    }

    public void AnchorNodeCoordinate(string myPanelName, Vector2Int myCoordinate)
    {
        _panelDictionary[myPanelName].GetNodeAt(myCoordinate.x, myCoordinate.y).isAnchored = true;
    }

    public void AnchorNodeIndex(string myPanelName, int index)
    {
        _panelDictionary[myPanelName].Nodes[index].isAnchored = true;
    }
    
    public static List<VerletNode> AllNodes;
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
        node.isAnchored = true;
        node.anchoredPos = myPosition;
    }

    public void FixedUpdate()
    {
        if (_mouseDragger.SelectedChildIndex != -1)
        {
            GetConnectedNodes();
            AllNodes[_mouseDragger.SelectedChildIndex].position = _mouseDragger.GetTargetPos();
        }

        if (_simConnected != null)
        {
            _simConnected.Simulate(5, Time.fixedDeltaTime);
        }

        if (_sim != null)
        {
            foreach (var node in _sim.Nodes)
            {
                if (node.isAnchored)
                {
                    node.position.y = node.anchoredPos.y;
                }
            }

            _sim.Simulate(2, Time.fixedDeltaTime);

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
            Debug.Log("sim connected");
        }

        if (_mouseDragger.HoveredChildIndex !=-1)
        {
            Gizmos.DrawSphere(AllNodes[_mouseDragger.HoveredChildIndex].position, 0.1f);
        }
    }

    public void RenderNodes(Material material, Mesh mesh)
    {
        var rparams = new RenderParams(material);
        List<Matrix4x4> renderMatrices = new();
        foreach (var node in IterateAllNodes())
        {
            Matrix4x4 mat = Matrix4x4.TRS(node.position, Quaternion.identity, Vector3.one * 0.1f);
            renderMatrices.Add(mat);
        }

        Graphics.RenderMeshInstanced(rparams, mesh, 0, renderMatrices);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Verlet;
using Vector2Int = UnityEngine.Vector2Int;

public class FabricManager
{

    private GarmentGenerator _parent;
    private Camera _camera;
    private MouseDragger _mouseDragger;

    public FabricManager(GarmentGenerator parent)
    {
        _parent = parent;
        _camera = Camera.main;
        _mouseDragger = new MouseDragger(_camera);
    }


    /*public float displacementFactor; // TO DO
    [SerializeField] private bool isRibbed; // TO DO*/
    private readonly Dictionary<string, PanelInfo> _panelDictionary = new();
    private readonly Dictionary<string, List<VerletNode>> _seamDictionary = new();
    private List<StitchInfo> _stitchInfos;
    private VerletSimulator _sim;
    private VerletSimulator _simConnected;
    public static int NodeCount;
    private List<VerletNode> _allNodes;
    private List<VerletNode> _anchoredNodes = new();
    private string _panelName;
    private bool _isCircular;
    private VerletNode closestNode;

    public void MakePanel(string mpName, int mpPanelWidth, int mpPanelHeight, bool mpIsCircular)
    {
        // get stitchInfos for panel (position, coordinates, etc.)
        _stitchInfos = new List<StitchInfo>(GridMaker.MakePanelWithParameters(
            new Vector2Int(mpPanelWidth, mpPanelHeight),
            new Vector2(_parent.stitchPrefab.width, _parent.stitchPrefab.height), mpIsCircular));

        // create stitchScripts at positions
        var initFabric = InitFabric(mpName, _stitchInfos);
        List<StitchScript> stitches = initFabric.Item1;
        GameObject parentObject = initFabric.Item2;

        // create verletnodes at positions
        List<VerletNode> nodes = InitNodes(_stitchInfos);

        //create panelInfo for panel
        var thisPanelInfo = new PanelInfo(stitches, nodes, mpPanelWidth, mpPanelHeight, mpIsCircular,
            parentObject.transform, parentObject.GetComponent<Panel>().heldStitchIndex);

        //add panelInfo to dictionary (to keep track of the panels we have, and be able to iterate through all of them)
        _panelDictionary[mpName] = thisPanelInfo;

        //connect stitches and nodes to the correct neighbors
        Connect(mpName);

        //pass all nodes to the verletsimulator
        _sim = new VerletSimulator(GetAllNodes());
    }

    public void RemovePreviousData()
    {

        foreach (var panelInfo in _panelDictionary.Values)
        {
            GameObject.Destroy(panelInfo.ParentObject.gameObject);
        }

        Debug.Log("remove data");
    }

    public PanelInfo GetPanelInfo(string key)
    {
        return _panelDictionary[key];
    }

    public List<VerletNode> GetSeams(string key)
    {
        return _seamDictionary[key];
    }

    public void ConnectSeams(string key1, string key2)
    {
        StitchConnector.ConnectSeams(_seamDictionary[key1], _seamDictionary[key2]);
    }

    (List<StitchScript>, GameObject) InitFabric(string myPanelName, List<StitchInfo> incomingStitchInfos)
    {
        var parentObject = new GameObject(myPanelName);
        parentObject.AddComponent<Panel>();
        var stitches = new List<StitchScript>();
        foreach (Transform child in parentObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (var stitch in incomingStitchInfos)
        {
            var newStitch = GameObject.Instantiate(_parent.stitchPrefab, stitch.Position, Quaternion.identity);
            newStitch.xCoordinate = stitch.XCoordinate;
            newStitch.yCoordinate = stitch.YCoordinate;
            // TO DO: create verlet nodes at stitch positions
            if (parentObject != null)
            {
                newStitch.transform.parent = parentObject.transform;
            }

            stitches.Add(newStitch);
        }

        return (stitches, parentObject);
    }

    List<VerletNode> InitNodes(List<StitchInfo> incomingStitchInfos)
    {
        var nodes = new List<VerletNode>();
        foreach (var stitch in incomingStitchInfos)
        {
            var newNode = new VerletNode(stitch.Position);
            newNode.initPos = stitch.Position;
            // TO DO: create verlet nodes at stitch positions
            nodes.Add(newNode);
        }

        return nodes;
    }

    private void Connect(string myPanelName)
    {
        var panel = _panelDictionary[myPanelName];

        StitchConnector.ConnectStitches(panel.Stitches, panel.Width, panel.IsCircular);
        StitchConnector.ConnectNodes(panel.Nodes, panel.Width, panel.IsCircular);
    }

    private void AddNodeToSeamFromCoordinate(string myPanelName, string mySeamKey, Vector2Int myCoordinate)
    {
        if (!_seamDictionary.ContainsKey(mySeamKey))
        {
            _seamDictionary[mySeamKey] = new List<VerletNode>();
        }

        _seamDictionary[mySeamKey].Add(_panelDictionary[myPanelName].GetNodeAt(myCoordinate.x, myCoordinate.y));
    }

    public void CreateSeam(string myPanelName, string mySeamKey, Vector2Int myStartingPoint, Vector2Int myEndPoint,
        int myLength)
    {
        if (!_seamDictionary.ContainsKey(mySeamKey))
        {
            _seamDictionary[mySeamKey] = new List<VerletNode>();
        }

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
        if (!_seamDictionary.ContainsKey(mySeamKey))
        {
            _seamDictionary[mySeamKey] = new List<VerletNode>();
        }

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

    public void GetStitchValue()
    {
        foreach (var i in _panelDictionary["patternPanel"].Stitches)
        {
            i.isKnit = _parent.pattern.GetStitch(i.xCoordinate, i.yCoordinate);
        }
    }

    //get all nodes
    private List<VerletNode> GetAllNodes()
    {
        List<VerletNode> nodesToSimulate = new List<VerletNode>();
        foreach (PanelInfo myPanelInfo in _panelDictionary.Values)
        {
            nodesToSimulate.AddRange(myPanelInfo.Nodes);
        }

        NodeCount = nodesToSimulate.Count;
        return nodesToSimulate;
    }

    public IEnumerable<VerletNode> IterateAllNodes()
    {
        foreach (PanelInfo info in _panelDictionary.Values)
        {
            foreach (var n in info.Nodes) yield return n;
        }
    }
    
    //get nodes connected to selected node by radius
    private void GetConnectedNodes()
    {
        var centralNode = GetAllNodes()[_parent.heldStitchIndex];
        List<VerletNode> connectedNodesLayer1 = new List<VerletNode>();
        List<VerletNode> connectedNodesLayer2 = new List<VerletNode>();
        List<VerletNode> connectedNodesLayer3 = new List<VerletNode>();
        foreach (var edge in centralNode.Connection)
        {
            if (!connectedNodesLayer1.Contains(edge.Other(centralNode)))
            {
                connectedNodesLayer1.Add(edge.Other(centralNode));
            }
        }

        connectedNodesLayer2.AddRange(connectedNodesLayer1);
        foreach (var node in connectedNodesLayer1)
        {
            foreach (var edge in node.Connection)
            {
                if (!connectedNodesLayer2.Contains(edge.Other(node)))
                {
                    connectedNodesLayer2.Add(edge.Other(node));
                }
            }
        }

        foreach (var node in connectedNodesLayer2)
        {
            foreach (var edge in node.Connection)
            {
                if (!connectedNodesLayer3.Contains(edge.Other(node)))
                {
                    connectedNodesLayer3.Add(edge.Other(node));
                }
            }
        }

        _simConnected = new VerletSimulator(connectedNodesLayer3);
    }

    public void AnchorNode(VerletNode node, Vector3 myPosition)
    {
        node.isAnchored = true;
        node.anchoredPos = myPosition;
    }

    public void FixedUpdate()
    {
        //hardcoded anchoring: large sweater
        
        /*_panelDictionary["sleeveLeft"].Nodes[_panelDictionary["sleeveLeft"].Width / 2].position.x = 0;
        _panelDictionary["sleeveRight"].Nodes[_panelDictionary["sleeveRight"].Width / 2].position.x =
            _panelDictionary["frontPanel"].Width * _parent.stitchPrefab.width;
        _panelDictionary["sleeveLeft"].Nodes[^(_panelDictionary["sleeveLeft"].Width / 2)].position.x =
            0 - _panelDictionary["sleeveLeft"].Height * _parent.stitchPrefab.height;
        _panelDictionary["sleeveRight"].Nodes[^(_panelDictionary["sleeveRight"].Width / 2)].position.x =
            _panelDictionary["frontPanel"].Width * _parent.stitchPrefab.width +
            _panelDictionary["sleeveRight"].Height * _parent.stitchPrefab.height;*/

        
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            var hoveredNode = _mouseDragger.ClosestChild(GetAllNodes());
            closestNode = hoveredNode != -1 ? GetAllNodes()[_mouseDragger.ClosestChild(GetAllNodes())] : null;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (closestNode != null)
            {
                closestNode.position = _mouseDragger.GetTargetPos();
            }
        }
        
        foreach (var panelinfo in _panelDictionary.Values)
        {
            panelinfo.Nodes[panelinfo.HeldStitchIndex].position = panelinfo.ParentObject.transform.position;
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
            _sim.DrawGizmos();
        }

        if (closestNode != null)
        {
            Gizmos.DrawSphere(closestNode.position, 0.1f);
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

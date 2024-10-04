using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Verlet;

public class FabricManager : MonoBehaviour
{
    
    [FormerlySerializedAs("StitchPrefab")] public StitchScript stitchPrefab;
    [FormerlySerializedAs("Pattern")] public Pattern pattern;
    /*public float displacementFactor; // TO DO
    [SerializeField] private bool isRibbed; // TO DO*/
    private Dictionary<string, PanelInfo> _panelDictionary = new();
    private List<StitchInfo> _stitchInfos;
    private VerletSimulator _sim;
    private List<VerletNode> _nodesToSimulate;
    private List<VerletNode> _connectedNodesToSimulate;
    private string _panelName;
    private readonly int _width = 10;
    private readonly int _height = 10;
    private bool _isCircular;

    public int HeldStitchIndex;
    
    
    
    public struct StitchInfo
    {
        public Vector3 Position { get; }
        public int XCoordinate { get; }
        public int YCoordinate { get; }

        // Constructor to initialize the position
        public StitchInfo(Vector3 position, int x, int y)
        {
            Position = position;
            XCoordinate = x;
            YCoordinate = y;
        }
    }
    public class PanelInfo
    {
        private List<StitchScript> _stitches;
        public List<StitchScript> Stitches => _stitches;
        private List<VerletNode> _nodes;
        public List<VerletNode> Nodes => _nodes;
        private int _width;
        public int Width => _width;
        private int _height;
        public int Height => _height;
        private bool _isCircular;
        public bool IsCircular => _isCircular;
        private Transform _parentObject;
        public Transform ParentObject => _parentObject;

        public PanelInfo(List<StitchScript> stitches,List<VerletNode> nodes, int width, int height, bool isCircular, Transform parentObject)
        {
            _stitches = stitches;
            _nodes = nodes;
            _width = width;
            _height = height;
            _isCircular = isCircular;
            _parentObject = parentObject;
        }

        public StitchScript GetStitchAt(int x, int y)
        {
            return _stitches[Calculation.GetIndexFromCoordinate(x, y, _width)];
        }
    }

    private void MakePanel(string mpName, int mpPanelWidth, int mpPanelHeight, bool mpIsCircular)
    {
        if (_panelDictionary.Keys.Contains(mpName))
        {
            Destroy(_panelDictionary[mpName].ParentObject.gameObject);
            _panelDictionary.Remove(mpName);
        }
        _stitchInfos = new List<StitchInfo>(GridMaker.MakePanelWithParameters(new Vector2Int(mpPanelWidth, mpPanelHeight),
            new Vector2(stitchPrefab.width, stitchPrefab.height), mpIsCircular));
        var initFabric = InitFabric(mpName, _stitchInfos);
        List<StitchScript> stitches = initFabric.Item1;
        GameObject parentObject = initFabric.Item2;
        List<VerletNode> nodes = InitNodes( _stitchInfos);
        var thisPanelInfo = new PanelInfo(stitches, nodes, mpPanelWidth,mpPanelHeight,mpIsCircular, parentObject.transform);
        _panelDictionary[mpName] = thisPanelInfo;
        Connect(mpName);
        _nodesToSimulate = new List<VerletNode>(SimulateNodes());
        Debug.Log("nodestosimulate:" + _nodesToSimulate.Count);
        _connectedNodesToSimulate = new List<VerletNode>(SimulateConnectedNodes());
        Debug.Log("connectednodes:" + _connectedNodesToSimulate.Count);
        _sim = new VerletSimulator(_connectedNodesToSimulate);
    }
    
    public void MakePanel()
    {
        MakePanel(_panelName,_width,_height,_isCircular);
    }
    
    [ContextMenu("Make sweater mesh")]
    public void MakeSweaterMesh()
    {   
        //front panel:
        MakePanel("frontPanel",10,10,false);
        /*//back panel:
        MakePanel("backPanel",10,10,false);
        //left sleeve:
        MakePanel("leftSleeve",8,10,true);
        MakePanel("rightSleeve",8,10,true);*/
        
        var frontPanel = _panelDictionary["frontPanel"];
        /*var backPanel = _panelDictionary["backPanel"];
        var leftSleeve = _panelDictionary["leftSleeve"];
        var rightSleeve = _panelDictionary["rightSleeve"];*/
        //takes a specific stitch from the list and sets it inactive (trying out to make neck shaping this way)
        frontPanel.GetStitchAt(5,5).gameObject.SetActive(false);
        
    }

    [ContextMenu("Make pattern mesh")]
    public void MakePanelFromPattern()
    {
        MakePanel("patternPanel", pattern.width,pattern.height,pattern.isCircular);
        /*var patternPanel = _panelDictionary["patternPanel"];*/
        // TO DO: connect stitches
        GetStitchValue();
    }

    (List<StitchScript>, GameObject) InitFabric(string myPanelName, List<StitchInfo> incomingStitchInfos)
    {
        var parentObject = new GameObject(myPanelName);
        var stitches = new List<StitchScript>();
        foreach (Transform child in parentObject.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var stitch in incomingStitchInfos)
        {
            var newStitch = Instantiate(stitchPrefab, stitch.Position, Quaternion.identity);
            // TO DO: create verlet nodes at stitch positions
            if(parentObject != null)
            {
                newStitch.transform.parent = parentObject.transform;
            }
            stitches.Add(newStitch);
        }
        return (stitches,parentObject) ;
    }
    
    List<VerletNode> InitNodes( List<StitchInfo> incomingStitchInfos)
    {
        var nodes = new List<VerletNode>();
        foreach (var stitch in incomingStitchInfos)
        {
            var newNode = new VerletNode(stitch.Position);
            // TO DO: create verlet nodes at stitch positions
            nodes.Add(newNode);
        }
        return nodes;
    }
    
    
     private void Connect(string myPanelName)
     {
         var panel = _panelDictionary[myPanelName];
        
        StitchConnector.ConnectStitches(panel.Stitches,panel.Width,panel.IsCircular);
        StitchConnector.ConnectNodes(panel.Nodes,panel.Width,panel.IsCircular);
     }
    private void GetStitchValue()
    {
        foreach (var i in _panelDictionary["patternPanel"].Stitches)
        {
            i.isKnit = pattern.GetStitch(i.xCoordinate, i.yCoordinate);
        }
    }
    
    
    
    //simulate
    private List<VerletNode> SimulateNodes()
    {
        List<VerletNode> nodesToSimulate = new List<VerletNode>();
        foreach (PanelInfo myPanelInfo in _panelDictionary.Values)
        {
            nodesToSimulate.AddRange(myPanelInfo.Nodes);
        }
        return nodesToSimulate;


    }

    private List<VerletNode> SimulateConnectedNodes()
    {
        var centralNode = SimulateNodes()[HeldStitchIndex];
        List<VerletNode> connectedNodesToSimulate = new List<VerletNode>();
        foreach (var edge in centralNode.Connection)
        {
            connectedNodesToSimulate.Add(edge.Other(centralNode));
        }
        Debug.Log(connectedNodesToSimulate.Count);
        return connectedNodesToSimulate;
    }

    private void FixedUpdate()
    {
        

        if (_sim != null)
        {
            _sim.Simulate(10, Time.fixedDeltaTime);
        }
        if (_nodesToSimulate != null)
        {
            foreach (var node in _nodesToSimulate)
            {
                node.position = node.initPos;
            }
        }
    }
}
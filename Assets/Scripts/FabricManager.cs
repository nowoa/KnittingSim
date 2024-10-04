using System.Collections.Generic;
using System.Linq;
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
    private VerletSimulator _simConnected;
    private List<VerletNode> _nodesToSimulate;
    private List<VerletNode> _connectedNodesToSimulate;
    private string _panelName;
    private readonly int _width = 10;
    private readonly int _height = 10;
    private bool _isCircular;
    
    public string panelName;
    public  int width = 10;
    public  int height = 10;
    public bool isCircular;

    [FormerlySerializedAs("HeldStitchIndex")] public int heldStitchIndex;
    
    
    
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

    private class PanelInfo
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
        //remove already created panels with the same name
        if (_panelDictionary.Keys.Contains(mpName))
        {
            Destroy(_panelDictionary[mpName].ParentObject.gameObject);
            _panelDictionary.Remove(mpName);
        }
        
        // get stitchInfos for panel (position, coordinates, etc.)
        _stitchInfos = new List<StitchInfo>(GridMaker.MakePanelWithParameters(new Vector2Int(mpPanelWidth, mpPanelHeight),
            new Vector2(stitchPrefab.width, stitchPrefab.height), mpIsCircular));
        
        // create stitchScripts at positions
        var initFabric = InitFabric(mpName, _stitchInfos);
        List<StitchScript> stitches = initFabric.Item1;
        GameObject parentObject = initFabric.Item2;
        
        // create verletnodes at positions
        List<VerletNode> nodes = InitNodes( _stitchInfos);
        
        
        //create panelInfo for panel
        var thisPanelInfo = new PanelInfo(stitches, nodes, mpPanelWidth,mpPanelHeight,mpIsCircular, parentObject.transform);
        
        //add panelInfo to dictionary (to keep track of the panels we have, and be able to iterate through all of them)
        _panelDictionary[mpName] = thisPanelInfo;
        
        //connect stitches and nodes to the correct neighbors
        Connect(mpName);
        
        //pass all nodes to the verletsimulator
        _sim = new VerletSimulator(GetAllNodes());
    }
    
    [ContextMenu("Make panel")]
    public void MakePanel()
    {
        MakePanel(panelName,width,height,isCircular);
    }
    
    [ContextMenu("Make sweater mesh")]
    public void MakeSweaterMesh()
    {   
        //front panel:
        MakePanel("frontPanel",10,10,false);
        //back panel:
        MakePanel("backPanel",10,10,false);
        //left sleeve:
        MakePanel("leftSleeve",8,10,true);
        MakePanel("rightSleeve",8,10,true);
        
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
    
    //get all nodes
    private List<VerletNode> GetAllNodes()
    {
        List<VerletNode> nodesToSimulate = new List<VerletNode>();
        foreach (PanelInfo myPanelInfo in _panelDictionary.Values)
        {
            nodesToSimulate.AddRange(myPanelInfo.Nodes);
        }
        return nodesToSimulate;
    }

    [ContextMenu("update connected node")]
    private void GetConnectedNodes()
    {
        var centralNode = GetAllNodes()[heldStitchIndex];
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

            
        Debug.Log(connectedNodesLayer3.Count);
        _simConnected = new VerletSimulator(connectedNodesLayer3);

    }

    private void FixedUpdate()
    {
        if (_sim != null)
        {
            _sim.Simulate(2, Time.fixedDeltaTime);
        }

        if (_simConnected != null)
        {
            _simConnected.Nodes[_simConnected.Nodes.Count / 2].position = transform.position;
            _simConnected.Simulate(5, Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (_sim != null)
        {
            _sim.DrawGizmos();
        }
    }
    
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    private Dictionary<string, List<VerletNode>> _seamDictionary = new();
    private List<StitchInfo> _stitchInfos;
    private VerletSimulator _sim;
    private VerletSimulator _simConnected;
    private List<VerletNode> _nodesToSimulate;
    private List<VerletNode> _connectedNodesToSimulate;
    private List<VerletNode> _anchoredNodes = new();
    private string _panelName;
    private readonly int _width = 10;
    private readonly int _height = 10;
    private bool _isCircular;
    
    public string panelName;
    public  int width = 10;
    public  int height = 10;
    public bool isCircular;
    public bool anchored;

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

        public VerletNode GetNodeAt(int x, int y)
        {
            return _nodes[Calculation.GetIndexFromCoordinate(x, y, _width)];
        }
    }

    private void Start()
    {
    }

    private void MakePanel(string mpName, int mpPanelWidth, int mpPanelHeight, bool mpIsCircular)
    {
        //remove previous data
        RemovePreviousData(mpName);
        
        
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

    void RemovePreviousData(string myPanelName)
    {
        if (_panelDictionary.Keys.Contains(myPanelName))
        {
            Destroy(_panelDictionary[myPanelName].ParentObject.gameObject);
            _panelDictionary.Remove(myPanelName);
        }

        if (_anchoredNodes != null)
        {
            _anchoredNodes.Clear();
        }

        if (_nodesToSimulate != null)
        {
            _nodesToSimulate.Clear();
        }

        if (_connectedNodesToSimulate != null)
        {
            _connectedNodesToSimulate.Clear();
        }
    }
    
    [ContextMenu("Make panel")]
    public void MakePanel()
    {
        MakePanel(panelName,width,height,isCircular);
        if (anchored)
        {
            for (int i = 0; i < width; i++)
            {
                AnchorNodeIndex(panelName,i);
            }
        }
    }
    
    [ContextMenu("Make sweater mesh")]
    public void MakeSweaterMesh()
    {
        var fp = "frontPanel";
        var fpnL = "frontPanelNeckLeft";
        var fpnR = "frontPanelNeckRight";
        var fpbL = "frontPanelBodyLeft";
        var fpbR = "frontPanelBodyRight";
        
        var bp = "backPanel";
        var bpnL = "backPanelNeckLeft";
        var bpnR = "backPanelNeckRight";
        var bpbL = "backPanelBodyLeft";
        var bpbR = "backPanelBodyRight";
        
        var sL = "sleeveLeft";
        var s2bL = "sleeveToBodyLeft";
        var sR = "sleeveRight";
        var s2bR = "sleeveToBodyRight";

        var b2sL = "bodyToSleeveLeft";
        var b2sR = "bodyToSleeveRight";
        //front panel:
        MakePanel(fp,10,10,false);
        //create seams:
        //left neck seam 
        CreateSeam(fp,fpnL,new Vector2Int(0,9),new Vector2Int(2,9),3);
        //right neck seam 
        /*AddNodeToSeamFromCoordinate(fp, fpnR, new Vector2Int(7,9) );
        AddNodeToSeamFromCoordinate(fp, fpnR, new Vector2Int(8,9) );
        AddNodeToSeamFromCoordinate(fp, fpnR, new Vector2Int(9,9) );*/
        CreateSeam(fp,fpnR,new Vector2Int(7,9),new Vector2Int(9,9),3);
        //left body seam 
        /*AddNodeToSeamFromCoordinate(fp, fpbL, new Vector2Int(0,0) );
        AddNodeToSeamFromCoordinate(fp, fpbL, new Vector2Int(0,1) );
        AddNodeToSeamFromCoordinate(fp, fpbL, new Vector2Int(0,2) );
        AddNodeToSeamFromCoordinate(fp, fpbL, new Vector2Int(0,3) );
        AddNodeToSeamFromCoordinate(fp, fpbL, new Vector2Int(0,4) );
        AddNodeToSeamFromCoordinate(fp, fpbL, new Vector2Int(0,5) );*/
        CreateSeam(fp,fpbL,new Vector2Int(0,0),new Vector2Int(0,5),6);
        //right body seam
        /*AddNodeToSeamFromCoordinate(fp, fpbR, new Vector2Int(9,0) );
        AddNodeToSeamFromCoordinate(fp, fpbR, new Vector2Int(9,1) );
        AddNodeToSeamFromCoordinate(fp, fpbR, new Vector2Int(9,2) );
        AddNodeToSeamFromCoordinate(fp, fpbR, new Vector2Int(9,3) );
        AddNodeToSeamFromCoordinate(fp, fpbR, new Vector2Int(9,4) );
        AddNodeToSeamFromCoordinate(fp, fpbR, new Vector2Int(9,5) );*/
        CreateSeam(fp,fpbR,new Vector2Int(9,0),new Vector2Int(9,5),6);
        //left sleeve seam (upwards)
        /*AddNodeToSeamFromCoordinate(fp, b2sL, new Vector2Int(0,6) );
        AddNodeToSeamFromCoordinate(fp, b2sL, new Vector2Int(0,7) );
        AddNodeToSeamFromCoordinate(fp, b2sL, new Vector2Int(0,8) );
        AddNodeToSeamFromCoordinate(fp, b2sL, new Vector2Int(0,9) );*/
        CreateSeam(fp,b2sL,new Vector2Int(0,6),new Vector2Int(0,9),4);
        //right sleeve seam
        /*AddNodeToSeamFromCoordinate(fp, b2sR, new Vector2Int(9,6) );
        AddNodeToSeamFromCoordinate(fp, b2sR, new Vector2Int(9,7) );
        AddNodeToSeamFromCoordinate(fp, b2sR, new Vector2Int(9,8) );
        AddNodeToSeamFromCoordinate(fp, b2sR, new Vector2Int(9,9) );*/
        CreateSeam(fp,b2sR,new Vector2Int(9,6),new Vector2Int(9,9),4);
        //back panel:
        MakePanel(bp,10,10,false);
        
        /*AddNodeToSeamFromCoordinate(bp, bpnL, new Vector2Int(0,9) );
        AddNodeToSeamFromCoordinate(bp, bpnL, new Vector2Int(1,9) );
        AddNodeToSeamFromCoordinate(bp, bpnL, new Vector2Int(2,9) );*/
        CreateSeam(bp,bpnL,new Vector2Int(0,9),new Vector2Int(2,9),3);
        /*AddNodeToSeamFromCoordinate(bp, bpnR, new Vector2Int(7,9) );
        AddNodeToSeamFromCoordinate(bp, bpnR, new Vector2Int(8,9) );
        AddNodeToSeamFromCoordinate(bp, bpnR, new Vector2Int(9,9) );*/
        CreateSeam(bp,bpnR,new Vector2Int(7,9),new Vector2Int(9,9),3);
        //left body seam 
        /*AddNodeToSeamFromCoordinate(bp, bpbL, new Vector2Int(0,0) );
        AddNodeToSeamFromCoordinate(bp, bpbL, new Vector2Int(0,1) );
        AddNodeToSeamFromCoordinate(bp, bpbL, new Vector2Int(0,2) );
        AddNodeToSeamFromCoordinate(bp, bpbL, new Vector2Int(0,3) );
        AddNodeToSeamFromCoordinate(bp, bpbL, new Vector2Int(0,4) );
        AddNodeToSeamFromCoordinate(bp, bpbL, new Vector2Int(0,5) );*/
        CreateSeam(bp,bpbL,new Vector2Int(0,0),new Vector2Int(0,5),6);
        //right body seam
        /*AddNodeToSeamFromCoordinate(bp, bpbR, new Vector2Int(9,0) );
        AddNodeToSeamFromCoordinate(bp, bpbR, new Vector2Int(9,1) );
        AddNodeToSeamFromCoordinate(bp, bpbR, new Vector2Int(9,2) );
        AddNodeToSeamFromCoordinate(bp, bpbR, new Vector2Int(9,3) );
        AddNodeToSeamFromCoordinate(bp, bpbR, new Vector2Int(9,4) );
        AddNodeToSeamFromCoordinate(bp, bpbR, new Vector2Int(9,5) );*/
        CreateSeam(bp,bpbR,new Vector2Int(9,0),new Vector2Int(9,5),6);
        
        //left sleeve seam (downwards)
        /*AddNodeToSeamFromCoordinate(bp, b2sL, new Vector2Int(0,8) );
        AddNodeToSeamFromCoordinate(bp, b2sL, new Vector2Int(0,7) );
        AddNodeToSeamFromCoordinate(bp, b2sL, new Vector2Int(0,6) );*/
        CreateSeamReverse(bp,b2sL,new Vector2Int(0,8),new Vector2Int(0,6),3);
        //right sleeve seam
        /*AddNodeToSeamFromCoordinate(bp, b2sR, new Vector2Int(9,8) );
        AddNodeToSeamFromCoordinate(bp, b2sR, new Vector2Int(9,7) );
        AddNodeToSeamFromCoordinate(bp, b2sR, new Vector2Int(9,6) );*/
        CreateSeamReverse(bp,b2sR,new Vector2Int(9,8),new Vector2Int(9,6),3);
        
        //left sleeve:
        MakePanel(sL,7,10,true);
        
        MakePanel(sR,7,10,true);
        
        //seam to body
        /*AddNodeToSeamFromCoordinate(sL,s2bL, new Vector2Int(0,0));
        AddNodeToSeamFromCoordinate(sL,s2bL, new Vector2Int(1,0));
        AddNodeToSeamFromCoordinate(sL,s2bL, new Vector2Int(2,0));
        AddNodeToSeamFromCoordinate(sL,s2bL, new Vector2Int(3,0));
        AddNodeToSeamFromCoordinate(sL,s2bL, new Vector2Int(4,0));
        AddNodeToSeamFromCoordinate(sL,s2bL, new Vector2Int(5,0));
        AddNodeToSeamFromCoordinate(sL,s2bL, new Vector2Int(6,0));*/
        CreateSeam(sL,s2bL,new Vector2Int(0,0),new Vector2Int(6,0),7);
        
        AddNodeToSeamFromCoordinate(sR,s2bR, new Vector2Int(0,0));
        AddNodeToSeamFromCoordinate(sR,s2bR, new Vector2Int(1,0));
        AddNodeToSeamFromCoordinate(sR,s2bR, new Vector2Int(2,0));
        AddNodeToSeamFromCoordinate(sR,s2bR, new Vector2Int(3,0));
        AddNodeToSeamFromCoordinate(sR,s2bR, new Vector2Int(4,0));
        AddNodeToSeamFromCoordinate(sR,s2bR, new Vector2Int(5,0));
        AddNodeToSeamFromCoordinate(sR,s2bR, new Vector2Int(6,0));
        
        
        var frontPanelInfo = _panelDictionary[fp];
        
        //takes a specific stitch from the list and sets it inactive (trying out to make neck shaping this way)
        frontPanelInfo.GetStitchAt(5,5).gameObject.SetActive(false);
        StitchConnector.ConnectSeams(_seamDictionary[fpnL],_seamDictionary[bpnL]);
        StitchConnector.ConnectSeams(_seamDictionary[fpnR],_seamDictionary[bpnR]);
        StitchConnector.ConnectSeams(_seamDictionary[fpbL],_seamDictionary[bpbL]);
        StitchConnector.ConnectSeams(_seamDictionary[fpbR],_seamDictionary[bpbR]);
        StitchConnector.ConnectSeams(_seamDictionary[s2bL],_seamDictionary[b2sL]);
        StitchConnector.ConnectSeams(_seamDictionary[s2bR],_seamDictionary[b2sR]);
        
        //add anchored nodes
        AnchorNodeCoordinate(fp, new Vector2Int(0,0));
        AnchorNodeCoordinate(fp, new Vector2Int(9,0));
        
        Debug.Log(b2sL.Length + " " + s2bL.Length);
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
            newStitch.xCoordinate = stitch.XCoordinate;
            newStitch.yCoordinate = stitch.YCoordinate;
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
            newNode.initPos = stitch.Position;
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

    private void AddNodeToSeamFromCoordinate(string myPanelName,string mySeamKey,  Vector2Int myCoordinate)
    {
        if (!_seamDictionary.ContainsKey(mySeamKey))
        {
            _seamDictionary[mySeamKey] = new List<VerletNode>();
        }
        _seamDictionary[mySeamKey].Add(_panelDictionary[myPanelName].GetNodeAt(myCoordinate.x,myCoordinate.y));
    }

    void CreateSeam(string myPanelName, string mySeamKey,Vector2Int myStartingPoint, Vector2Int myEndPoint, int myLength)
    {
        if (!_seamDictionary.ContainsKey(mySeamKey))
        {
            _seamDictionary[mySeamKey] = new List<VerletNode>();
        }
        float lerpStep = 1 / (float)myLength;
        for (int i = 1; i <= myLength; i++)
        {
            int x = (int)Mathf.Floor(Mathf.Lerp(myStartingPoint.x, myEndPoint.x, lerpStep*i));
            int y = (int)Mathf.Floor(Mathf.Lerp(myStartingPoint.y, myEndPoint.y, lerpStep*i));
            
            _seamDictionary[mySeamKey].Add(_panelDictionary[myPanelName].GetNodeAt(x,y));
            Debug.Log(mySeamKey + i + ": "+x + " " + y);
            Debug.Log(Mathf.Lerp(myStartingPoint.y, myEndPoint.y, lerpStep*i));
        }
        
    }
    void CreateSeamReverse(string myPanelName, string mySeamKey,Vector2Int myStartingPoint, Vector2Int myEndPoint, int myLength)
    {
        if (!_seamDictionary.ContainsKey(mySeamKey))
        {
            _seamDictionary[mySeamKey] = new List<VerletNode>();
        }
        float lerpStep = 1 / (float)myLength;
        for (int i = 1; i <= myLength; i++)
        {
            int x = (int)Mathf.Ceil(Mathf.Lerp(myStartingPoint.x, myEndPoint.x, lerpStep*i));
            int y = (int)Mathf.Ceil(Mathf.Lerp(myStartingPoint.y, myEndPoint.y, lerpStep*i));
            
            _seamDictionary[mySeamKey].Add(_panelDictionary[myPanelName].GetNodeAt(x,y));
            Debug.Log(mySeamKey + i + ": "+x + " " + y);
        }
        
    }

    private void AnchorNodeCoordinate(string myPanelName,Vector2Int myCoordinate)
    {
       _panelDictionary[myPanelName].GetNodeAt(myCoordinate.x, myCoordinate.y).isAnchored = true;
    }

    private void AnchorNodeIndex(string myPanelName, int index)
    {
        _panelDictionary[myPanelName].Nodes[index].isAnchored = true;
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
        _simConnected = new VerletSimulator(connectedNodesLayer3);

    }

    private void FixedUpdate()
    {
        foreach (var panelInfo in _panelDictionary.Values)
        {
            panelInfo.Nodes[65].position = panelInfo.ParentObject.position;
        }
        if (_simConnected != null)
        {
            
            _simConnected.Simulate(5, Time.fixedDeltaTime);
        }
        if (_sim != null)
        {
            _sim.Simulate(2, Time.fixedDeltaTime);
            foreach (var node in _sim.Nodes)
            {
                if (node.isAnchored)
                {
                    node.position = node.initPos;
                }
            }
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
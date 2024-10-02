using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FabricManager : MonoBehaviour
{
    
    [FormerlySerializedAs("StitchPrefab")] public StitchScript stitchPrefab;
    [FormerlySerializedAs("Pattern")] public Pattern pattern;
    /*public float displacementFactor; // TO DO
    [SerializeField] private bool isRibbed; // TO DO*/
    private Dictionary<string, PanelInfo> _panelDictionary;
    private List<StitchInfo> _stitchInfos;

    public string panelName;
    public int width = 10;
    public int height = 10;
    public bool isCircular;
    
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
        private int _width;
        public int Width => _width;
        private int _height;
        public int Height => _height;
        private bool _isCircular;
        public bool IsCircular => _isCircular;

        public PanelInfo(List<StitchScript> stitches, int width, int height, bool isCircular)
        {
            _stitches = stitches;
            _width = width;
            _height = height;
            _isCircular = isCircular;
        }
    }
    
    [ContextMenu("Make panel")]
    public void MakePanel(string mpName, int mpPanelWidth, int mpPanelHeight, bool mpIsCircular)
    {
        _stitchInfos = new List<StitchInfo>(GridMaker.MakePanelWithParameters(new Vector2Int(mpPanelWidth, mpPanelHeight),
            new Vector2(stitchPrefab.width, stitchPrefab.height), mpIsCircular));
        List<StitchScript> stitches = InitFabric(mpName, _stitchInfos);
        var thisPanelInfo = new PanelInfo(stitches, mpPanelWidth,mpPanelHeight,mpIsCircular);
        _panelDictionary[mpName] = thisPanelInfo;
        Connect(mpName);
    }
    
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
        frontPanel.Stitches[Calculation.GetIndexFromCoordinate(5, 5, frontPanel.Width)].gameObject.SetActive(false);
    }

    [ContextMenu("Make pattern mesh")]
    public void MakePanelFromPattern()
    {
        MakePanel("patternPanel", pattern.width,pattern.height,pattern.isCircular);
        /*var patternPanel = _panelDictionary["patternPanel"];*/
        // TO DO: connect stitches
        GetStitchValue();
    }
    List<StitchScript> InitFabric(string myPanelName, List<StitchInfo> incomingStitchInfos)
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
        return stitches;
    }
     private void Connect(string myPanelName)
     {
         var panel = _panelDictionary[myPanelName];
        StitchConnector.ConnectStitches(panel.Stitches,panel.Width,panel.IsCircular);
     }
    private void GetStitchValue()
    {
        foreach (var i in _panelDictionary["patternPanel"].Stitches)
        {
            i.isKnit = pattern.GetStitch(i.xCoordinate, i.yCoordinate);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class GarmentGenerator : MonoBehaviour
{
    private FabricManager _fabricManager;

    #region Parameters

    public Material material;
    public Mesh mesh;
    public Pattern pattern;
    public string panelName;
    public  int width = 10;
    public  int height = 10;
    public bool isCircular;
    public bool anchored;
    public int heldStitchIndex;
    public StitchScript stitchPrefab;
    
    public int bodyWidth = 20;
    public int bodyHeight = 40;
    public int sleeveWidth = 8;
    public int sleeveHeight = 50;
    public int collarWidth = 10;
    public int collarHeight = 2;

    #endregion

    private void Start()
    {
        UIManager.Instance.SetCamera(Camera.main);
    }

    [ContextMenu("make large sweater")]
    public void MakeLargeSweaterMesh()
    {
        Generate();
        var fp = "frontPanel";
        var fpnL = "frontPanelNeckLeft";
        var fpnR = "frontPanelNeckRight";
        var fpbL = "frontPanelBodyLeft";
        var fpbR = "frontPanelBodyRight";
        var b2c = "bodyToCollar";
        
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
        
        var cp = "collarPanel";
        var c2b = "collarPanelToBody";


        
        
        
        //front panel:
        _fabricManager.MakePanel(fp,bodyWidth,bodyHeight,false);
        _fabricManager.MakePanel(bp,bodyWidth,bodyHeight,false);
        //create seams:
        _fabricManager.CreateSeam(fp,fpnL,new Vector2Int(0,bodyHeight-1),new Vector2Int((bodyWidth-collarWidth)/2,bodyHeight-1),(bodyWidth-collarWidth)/2+1);
        _fabricManager.CreateSeam(bp,bpnL,new Vector2Int(0,bodyHeight-1),new Vector2Int((bodyWidth-collarWidth)/2,bodyHeight-1),(bodyWidth-collarWidth)/2+1);
        // x:0 y:height-1
        // x: ((width - collarwidth)/2), y: height-1
        // length=(width-collarwidth)/2
        _fabricManager.CreateSeam(fp,fpnR,new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-1,bodyHeight-1),new Vector2Int(bodyWidth-1,bodyHeight-1),
            (bodyWidth-collarWidth)/2+1);
        _fabricManager.CreateSeam(bp,bpnR,new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-1,bodyHeight-1),new Vector2Int(bodyWidth-1,bodyHeight-1),
            (bodyWidth-collarWidth)/2+1);
        //x:(width-((width-collarwidth)/2)) y: height-1
        //x:width-1 y:height-1
        //length=(width-collarwidth)/2
        _fabricManager.CreateSeam(fp,fpbL,new Vector2Int(0,0),new Vector2Int(0,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        _fabricManager.CreateSeam(bp,bpbL,new Vector2Int(0,0),new Vector2Int(0,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        //x:0,y:0
        //x:0,y:(height-sleevewidth)-1
        //length=height-sleevewidth
        _fabricManager.CreateSeam(fp,fpbR,new Vector2Int(bodyWidth-1,0),new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        _fabricManager.CreateSeam(bp,bpbR,new Vector2Int(bodyWidth-1,0),new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        //x:width-1,y:0
        //x:width-1.y:height-(sleevewidth)-1
        //length=height-sleevewidth
        _fabricManager.CreateSeam(fp,b2sL,new Vector2Int(0,bodyHeight-sleeveWidth),new Vector2Int(0,bodyHeight-1),sleeveWidth);
        _fabricManager.CreateSeamReverse(bp,b2sL,new Vector2Int(0,bodyHeight-2),new Vector2Int(0,bodyHeight-sleeveWidth-1),sleeveWidth);
        //x:0,y:height-sleevewidth
        //x:0,y:height-1
        //length:sleevewidth
        _fabricManager.CreateSeam(fp,b2sR,new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth),new Vector2Int(bodyWidth-1,bodyHeight-1),sleeveWidth);
        _fabricManager.CreateSeamReverse(bp,b2sR,new Vector2Int(bodyWidth-1,bodyHeight-2),new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth-1),sleeveWidth);
        //x:width-1,y:height-sleevewidth
        //x:width-1,y:height-1
        //length:sleevewidth
        _fabricManager.CreateSeam(fp,b2c,new Vector2Int((bodyWidth-collarWidth)/2,bodyHeight-1),new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-2,bodyHeight-1),collarWidth-1);
        _fabricManager.CreateSeamReverse(bp,b2c,new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-1,bodyHeight-1),new Vector2Int(((bodyWidth-collarWidth)/2)+1,bodyHeight-1),collarWidth-1);
        //x:(width - collarwidth)/2,y:height-1
        //x:width-((width-collarwidth)/2)-1,y:height-1 
        //length:collarwidth
        
        //back panel:
        
       
        // x:0 y:height-1
        // x: ((width - collarwidth)/2)-1, y: height-1
        // length=(width-collarwidth)/2
       
        //x:(width-((width-collarwidth)/2)) y: height-1
        //x:width-1 y:height-1
        //length=(width-collarwidth)/2
       
        //x:0,y:0
        //x:0,y:(height-sleevewidth)-1
        //length=height-sleevewidth
        
        //x:width-1,y:0
        //x:width-1.y:height-(sleevewidth)-1
        //length=height-sleevewidth
       
        //x:0,y:height-1
        //x:0,y:height-sleevewidth
        //length: sleevewidth
       
        //x:width-1,y:height-1
        //x:width-1,y:height-sleevewidth
        //length: sleevewidth
        
        //x:width-((width-collarwidth)/2)-1,y:height-1
        //x:(width-collarwidth)/2,y:height-1
        //length: collarwidth
        
        //left sleeve:
        _fabricManager.MakePanel(sL,sleeveWidth*2,sleeveHeight,true);
        //width: sleevewidth*2-2
        _fabricManager.MakePanel(sR,sleeveWidth*2,sleeveHeight,true);
        _fabricManager.CreateSeam(sL,s2bL,new Vector2Int(0,0),new Vector2Int(sleeveWidth*2-1,0),sleeveWidth*2);
        //x:0,y:0
        //x:sleevewidth*2-3,y:0
        //length:sleevewidth*2-2
        _fabricManager.CreateSeam(sR,s2bR,new Vector2Int(0,0),new Vector2Int(sleeveWidth*2-1,0),sleeveWidth*2);
        //x:0,y:0
        //x:sleevewidth*2-3,y:0
        //length:sleevewidth*2-2
        //make collar:
        _fabricManager.MakePanel(cp,collarWidth*2-2,collarHeight,true);
        //width:collarwidth*2-2
        _fabricManager.CreateSeam(cp,c2b,new Vector2Int(0,0),new Vector2Int(collarWidth*2-3,0),collarWidth*2-2);
        //x:0,y:0
        //x:collarwidth*2-3,y:0
        //length:collarwidth*2-2
        _fabricManager.ConnectSeams(fpnL,bpnL);
        _fabricManager.ConnectSeams(fpnR,bpnR);
        _fabricManager.ConnectSeams(fpbL,bpbL);
        _fabricManager.ConnectSeams(fpbR,bpbR);
        _fabricManager.ConnectSeams(s2bL,b2sL);
        _fabricManager.ConnectSeams(s2bR,b2sR);
        _fabricManager.ConnectSeams(c2b,b2c);

        foreach (var node in _fabricManager.GetSeams(fpnL))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }
        foreach (var node in _fabricManager.GetSeams(fpnR))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }
        foreach (var node in _fabricManager.GetSeams(bpnR))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }
        foreach (var node in _fabricManager.GetSeams(bpnL))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }

        for (int i = _fabricManager.GetPanelInfo(cp).Width; i > 0; i--)
        {
            _fabricManager.AnchorNode(_fabricManager.GetPanelInfo(cp).Nodes[^i],new Vector3(0,_fabricManager.GetPanelInfo(cp).Height*stitchPrefab.height,0) );
        }
    }
    
    [ContextMenu("make ruffle")]
    public void MakeRuffle()
    {
        Generate();
        _fabricManager.MakePanel("shortPanel",30,10,false);
        _fabricManager.MakePanel("longPanel",60,10,false);
        _fabricManager.CreateSeam("shortPanel", "shortPanelSeam", new Vector2Int(0,0), new Vector2Int(29,0),60);
        _fabricManager.CreateSeam("longPanel", "longPanelSeam", new Vector2Int(0,0), new Vector2Int(59,0),60);
        _fabricManager.CreateSeam("shortPanel","shortPanelAnchored",new Vector2Int(0,9), new Vector2Int(29,9),10);
        _fabricManager.ConnectSeams("shortPanelSeam", "longPanelSeam");
        foreach (var node in _fabricManager.GetSeams("shortPanelAnchored"))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }
    }
    
    [ContextMenu("Make pattern mesh")]
    public void MakePanelFromPattern()
    {
        Generate();
        _fabricManager.MakePanel("patternPanel", pattern.width,pattern.height,pattern.isCircular);
        /*var patternPanel = _panelDictionary["patternPanel"];*/
        // TO DO: connect stitches
        _fabricManager.GetStitchValue();
    }
    
    [ContextMenu("Make panel")]
    public void MakePanel()
    {
        Generate();
        _fabricManager.MakePanel(panelName,width,height,isCircular);
        if (anchored)
        {
            for (int i = 0; i < width; i++)
            {
                _fabricManager.AnchorNodeIndex(panelName,i);
            }
        }
        
    }

    void Generate()
    {
        _fabricManager?.RemovePreviousData();
        _fabricManager = new FabricManager(this);
    }
    
    private void FixedUpdate()
    {
        _fabricManager?.FixedUpdate();
    }

    private void OnDrawGizmos()
    {
        _fabricManager?.DrawGizmos();
    }

    private void Update()
    {
        _fabricManager?.RenderNodes(material, mesh);
    }
}

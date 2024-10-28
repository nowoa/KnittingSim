using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class GarmentGenerator : MonoBehaviour
{
    private FabricManager _fabricManager;
    private StitchTemplate _stitchTemplate = new();
    public StitchTemplate StitchTemplate => _stitchTemplate;

    #region Parameters

    public Material material;
    public Mesh mesh;
    public Pattern pattern;
    public string panelName;
    public  int width = 10;
    public  int height = 10;
    public bool isCircular;
    
    public int bodyWidth = 20;
    public int bodyHeight = 40;
    public int sleeveWidth = 8;
    public int sleeveHeight = 50;
    public int collarWidth = 10;
    public int collarHeight = 2;

    public int hGauge;
    public int vGauge;
    private float scaleFactor = 0.5f;

    #endregion

    

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

        _fabricManager.MakePanel(fp,bodyWidth,bodyHeight,false);
        _fabricManager.MakePanel(bp,bodyWidth,bodyHeight,false);
        _fabricManager.MakePanel(sL,sleeveWidth*2,sleeveHeight,true);
        _fabricManager.MakePanel(sR,sleeveWidth*2,sleeveHeight,true);
        _fabricManager.MakePanel(cp,collarWidth*2-2,collarHeight,true);
        
        _fabricManager.CreateSeam(fp,fpnL,new Vector2Int(0,bodyHeight-1),new Vector2Int((bodyWidth-collarWidth)/2,bodyHeight-1),(bodyWidth-collarWidth)/2+1);
        _fabricManager.CreateSeam(bp,bpnL,new Vector2Int(0,bodyHeight-1),new Vector2Int((bodyWidth-collarWidth)/2,bodyHeight-1),(bodyWidth-collarWidth)/2+1);
        _fabricManager.CreateSeam(fp,fpnR,new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-1,bodyHeight-1),new Vector2Int(bodyWidth-1,bodyHeight-1),
            (bodyWidth-collarWidth)/2+1);
        _fabricManager.CreateSeam(bp,bpnR,new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-1,bodyHeight-1),new Vector2Int(bodyWidth-1,bodyHeight-1),
            (bodyWidth-collarWidth)/2+1);
        _fabricManager.CreateSeam(fp,fpbL,new Vector2Int(0,0),new Vector2Int(0,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        _fabricManager.CreateSeam(bp,bpbL,new Vector2Int(0,0),new Vector2Int(0,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        _fabricManager.CreateSeam(fp,fpbR,new Vector2Int(bodyWidth-1,0),new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        _fabricManager.CreateSeam(bp,bpbR,new Vector2Int(bodyWidth-1,0),new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth-1),bodyHeight-sleeveWidth);
        _fabricManager.CreateSeam(fp,b2sL,new Vector2Int(0,bodyHeight-sleeveWidth),new Vector2Int(0,bodyHeight-1),sleeveWidth);
        _fabricManager.CreateSeam(bp,b2sL,new Vector2Int(0,bodyHeight-2),new Vector2Int(0,bodyHeight-sleeveWidth-1),sleeveWidth, true);
        _fabricManager.CreateSeam(fp,b2sR,new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth),new Vector2Int(bodyWidth-1,bodyHeight-1),sleeveWidth, true);
        _fabricManager.CreateSeam(bp,b2sR,new Vector2Int(bodyWidth-1,bodyHeight-2),new Vector2Int(bodyWidth-1,bodyHeight-sleeveWidth-1),sleeveWidth);
        _fabricManager.CreateSeam(fp,b2c,new Vector2Int((bodyWidth-collarWidth)/2,bodyHeight-1),new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-2,bodyHeight-1),collarWidth-1);
        _fabricManager.CreateSeam(bp,b2c,new Vector2Int(bodyWidth-((bodyWidth-collarWidth)/2)-1,bodyHeight-1),new Vector2Int(((bodyWidth-collarWidth)/2)+1,bodyHeight-1),collarWidth-1, true);
        _fabricManager.CreateSeam(sL,s2bL,new Vector2Int(0,0),new Vector2Int(sleeveWidth*2-1,0),sleeveWidth*2);
        _fabricManager.CreateSeam(sR,s2bR,new Vector2Int(0,0),new Vector2Int(sleeveWidth*2-1,0),sleeveWidth*2);
        _fabricManager.CreateSeam(cp,c2b,new Vector2Int(0,0),new Vector2Int(collarWidth*2-3,0),collarWidth*2-2);
        
        _fabricManager.ConnectSeams(fpnL,bpnL);
        _fabricManager.ConnectSeams(fpnR,bpnR);
        _fabricManager.ConnectSeams(fpbL,bpbL);
        _fabricManager.ConnectSeams(fpbR,bpbR);
        _fabricManager.ConnectSeams(s2bL,b2sL);
        _fabricManager.ConnectSeams(s2bR,b2sR);
        _fabricManager.ConnectSeams(c2b,b2c);

        /*foreach (var node in _fabricManager.GetSeam(fpnL))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }
        foreach (var node in _fabricManager.GetSeam(fpnR))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }
        foreach (var node in _fabricManager.GetSeam(bpnR))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }
        foreach (var node in _fabricManager.GetSeam(bpnL))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }

        for (int i = _fabricManager.GetPanelInfo(cp).Width; i > 0; i--)
        {
            _fabricManager.AnchorNode(_fabricManager.GetPanelInfo(cp).Nodes[^i],new Vector3(0,_fabricManager.GetPanelInfo(cp).Height*_stitchTemplate.height,0) );
        }*/
    }
    
    [ContextMenu("make ruffle")]
    public void MakeRuffle()
    {
        Generate();
        _fabricManager.MakePanel("shortPanel",30,10,false);
        _fabricManager.MakePanel("longPanel",60,10,false);
        _fabricManager.CreateSeam("shortPanel", "shortPanelSeam", new Vector2Int(0,0), new Vector2Int(29,0),60);
        _fabricManager.CreateSeam("longPanel", "longPanelSeam", new Vector2Int(0,0), new Vector2Int(59,0),60);
        /*_fabricManager.CreateSeam("shortPanel","shortPanelAnchored",new Vector2Int(0,9), new Vector2Int(29,9),30);*/
        _fabricManager.ConnectSeams("shortPanelSeam", "longPanelSeam");
        /*foreach (var node in _fabricManager.GetSeam("shortPanelAnchored"))
        {
            _fabricManager.AnchorNode(node,new Vector3(0,0,0));
        }*/
    }

    [ContextMenu("make scrunchie")]
    public void MakeScrunchie()
    {
        Generate();
        var ruffleWidth = 20;
        var ruffleHeight = 7;
        var hairTieWidth = 7;
        
        _fabricManager.MakePanel("ruffles",ruffleWidth,ruffleHeight,true);
        _fabricManager.MakePanel("hairTie",hairTieWidth,1,true);
        
        _fabricManager.CreateSeam("ruffles", "rufflesToHairTieBottom", new Vector2Int(0,0), new Vector2Int(ruffleWidth-1,0),ruffleWidth);
        _fabricManager.CreateSeam("ruffles", "rufflesToHairTieTop", new Vector2Int(0,ruffleHeight-1), new Vector2Int(ruffleWidth-1,ruffleHeight-1),ruffleWidth);
        _fabricManager.CreateSeam("hairTie", "hairTieToRuffles", new Vector2Int(0,0),new Vector2Int(hairTieWidth-1,0),ruffleWidth);
        
        _fabricManager.ConnectSeams("rufflesToHairTieBottom", "hairTieToRuffles");
        _fabricManager.ConnectSeams("rufflesToHairTieTop", "hairTieToRuffles");
        /*
        _fabricManager.AnchorNode(_fabricManager.GetSeam("hairTieToRuffles")[0],new Vector3(0,0,0));
        _fabricManager.AnchorNode(_fabricManager.GetSeam("hairTieToRuffles")[1],new Vector3(0,0,0));
        _fabricManager.AnchorNode(_fabricManager.GetSeam("hairTieToRuffles")[2],new Vector3(0,0,0));*/
    }
    
    [ContextMenu("Make panel")]
    public void MakePanel()
    {
        Generate();
        _fabricManager.MakePanel(panelName,width,height,isCircular);
        
    }

    void Generate()
    {
        //set stitch size to gauge
        StitchTemplate.height = (10f / vGauge)*scaleFactor;
        StitchTemplate.width = (10f / hGauge)*scaleFactor;
        
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

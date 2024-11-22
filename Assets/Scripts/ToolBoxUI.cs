using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBoxUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter()
    {
        InputHandler.GameInput = false;
    }

    public void OnPointerExit()
    {
        InputHandler.GameInput = true;
    }

    public void Dragger()
    {
        ToolManager.SetActiveTool(ToolManager.Dragger);
    }

    public void StitchBrush()
    {
        ToolManager.SetActiveTool(ToolManager.StitchBrush);
    }

    public void Increaser()
    {
        ToolManager.SetActiveTool(ToolManager.Increaser);
    }

    public void Decreaser()
    {
        ToolManager.SetActiveTool(ToolManager.Decreaser);
    }

    public void PanelStamp()
    {
        ToolManager.SetActiveTool(ToolManager.PanelStamp);
    }

    public void SeamMaker()
    {
        ToolManager.SetActiveTool(ToolManager.SeamMaker);
    }

    public void Knife()
    {
        ToolManager.SetActiveTool(ToolManager.Knife);
    }

    public void MeshSnapshot()
    {
        var meshScript = GameObject.FindObjectOfType<MeshManager>();
        meshScript.GenerateMesh();
    }
}

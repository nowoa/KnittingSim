using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

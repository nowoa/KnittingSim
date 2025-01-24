using UnityEngine;
using UnityEngine.UI;
using Verlet;

public class ToolBoxUI : MonoBehaviour
{
    public Slider slider;
    
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
        ToolManager.SetActiveTool(ToolManager.DraggerInstance);
    }

    public void StitchBrush()
    {
        ToolManager.SetActiveTool(ToolManager.StitchBrushInstance);
    }

    /*
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

    public void SeamTool()
    {
        ToolManager.SetActiveTool(ToolManager.SeamTool);
    }

    public void Knife()
    {
        ToolManager.SetActiveTool(ToolManager.Knife);
    }

    public void MeshSnapshot()
    {
        var meshScript = GameObject.FindObjectOfType<FabricMesh>();
        meshScript.UpdateMesh();
    }*/

    public void ToggleHover()
    {
        GameManager.Instance.Hover.IsActive = !GameManager.Instance.Hover.IsActive;
        Debug.Log(GameManager.Instance.Hover.IsActive);
    }

    public void CreatePanel()
    {
        PanelGenerator.GeneratePanel(Random.value.ToString(), new Vector2Int(Mathf.FloorToInt(slider.value), Mathf.FloorToInt(slider.value)),false, new Vector2Int(30,30));
    }

    public void CollisionToggle()
    {
        GameManager.Instance.Project.Collision = !GameManager.Instance.Project.Collision;
    }

    public void HashGridOverlapToggle()
    {
        SelfCollision.checkDouble = !SelfCollision.checkDouble;
    }

}

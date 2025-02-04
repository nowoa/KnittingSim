using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Verlet;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ToolBoxUI : MonoBehaviour
{
    public static ToolBoxUI Instance;
    
    public Slider slider;
    public Slider Radius;
    [FormerlySerializedAs("MouseRadiusCircle")] public Image mouseRadiusCircle;

    private void Awake()
    {
        Instance = this;
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
        ToolManager.SetActiveTool(ToolManager.DraggerInstance);
        SetMouseRadius(Radius.value);
    }

    public void StitchBrush()
    {
        ToolManager.SetActiveTool(ToolManager.StitchBrushInstance);
        SetMouseRadius(Radius.value);
    }
    
    public void ColorBrush()
    {
        ToolManager.SetActiveTool(ToolManager.ColorBrushInstance);
        SetMouseRadius(Radius.value);
    }

    public void Selector()
    {
        ToolManager.SetActiveTool(ToolManager.SelectorInstance);
        SetMouseRadius(50f);
    }

    public void Frog()
    {
        GameManager.Instance.Project.FrogAll();
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
        PanelGenerator.GeneratePanel(Random.value.ToString(), new Vector2Int(Mathf.FloorToInt(slider.value), Mathf.FloorToInt(slider.value)),false, new Vector2Int(30,30), new Vector3(0,0,0));
    }

    public void CollisionToggle()
    {
        GameManager.Instance.Project.Collision = !GameManager.Instance.Project.Collision;
    }

    public void HashGridOverlapToggle()
    {
        SelfCollision.checkDouble = !SelfCollision.checkDouble;
    }

    public void UpdateRadiusSlider(float radius)
    {
        SetMouseRadius(radius);
    }


    public void SetMouseRadius(float radius)
    {
        GameManager.Instance.Hover.MouseRadius = radius;
    }

}

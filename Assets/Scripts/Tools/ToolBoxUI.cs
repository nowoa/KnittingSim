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
    }

    public void StitchBrush()
    {
        ToolManager.SetActiveTool(ToolManager.StitchBrushInstance);
    }
    
    public void ColorBrush()
    {
        ToolManager.SetActiveTool(ToolManager.ColorBrushInstance);
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


    public void SetMouseRadius(float radius)
    {/*
        if (ToolManager.ActiveTool == ToolManager.DraggerInstance)
        {
            GameManager.Instance.Hover.AnchoredNodeRadius = radius;
            GameManager.Instance.Hover.MouseRadius = 50f;
        }
        else */GameManager.Instance.Hover.MouseRadius = radius;
        mouseRadiusCircle.rectTransform.localScale = new Vector3(radius / 50, radius / 50, radius / 50);
    }

    public void UpdateUI()
    {
        mouseRadiusCircle.rectTransform.position = Input.mousePosition;
    }

}

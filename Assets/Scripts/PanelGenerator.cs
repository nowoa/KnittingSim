using UnityEngine;

public class PanelGenerator : MonoBehaviour
{
    public string panelName;
    public Vector2Int dimensions;
    public bool isCircular;
    public Vector2Int gauge;

    [ContextMenu("Generate panel")]
    public static void GeneratePanel(string panelName, Vector2Int dimensions, bool isCircular, Vector2Int gauge)
    {
        GameManager.Instance.Project.AddPanel(panelName,dimensions, isCircular, gauge);
    }
}

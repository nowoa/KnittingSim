using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGenerator : MonoBehaviour
{
    public string panelName;
    public Vector2Int dimensions;
    public bool isCircular;
    public Vector2Int gauge;

    [ContextMenu("Generate panel")]
    private void GeneratePanel()
    {
        GameManager.Instance.Project.AddPanel(panelName,dimensions, isCircular, gauge);
    }
}

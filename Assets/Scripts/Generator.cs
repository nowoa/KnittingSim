using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Verlet;
using Vector3 = UnityEngine.Vector3;

public class PanelGenerator : MonoBehaviour
{
    public string panelName;
    public Vector2Int dimensions;
    public bool isCircular;
    public Vector2Int gauge;

    [ContextMenu("Generate panel")]
    private void GeneratePanel()
    {
        var panelConfig = 
            new PanelConfig(
                panelName,
                dimensions,
                isCircular,
                gauge,
                Vector3.zero);
        GameManager.Instance.Project.AddPanel(panelConfig);
    }
    
    public static void GeneratePanel(PanelConfig panelConfig)
    {
        GameManager.Instance.Project.AddPanel(panelConfig);
    }
}



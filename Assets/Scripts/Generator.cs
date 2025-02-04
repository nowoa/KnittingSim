using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

public class PanelGenerator : MonoBehaviour
{
    public string panelName;
    public Vector2Int dimensions;
    public bool isCircular;
    public Vector2Int gauge;

    [ContextMenu("Generate panel")]
    private void GeneratePanel()
    {
        GameManager.Instance.Project.AddPanel(panelName,dimensions, isCircular, gauge, new Vector3(0,0,0));
    }
    
    public static void GeneratePanel(string myPanelName, Vector2Int myDimensions, bool myIsCircular, Vector2Int myGauge, Vector3 startPos)
    {
        GameManager.Instance.Project.AddPanel(myPanelName,myDimensions, myIsCircular, myGauge, startPos);
    }
}



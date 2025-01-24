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
    
    public static void GeneratePanel(string myPanelName, Vector2Int myDimensions, bool myIsCircular, Vector2Int myGauge)
    {
        GameManager.Instance.Project.AddPanel(myPanelName,myDimensions, myIsCircular, myGauge);
    }
}

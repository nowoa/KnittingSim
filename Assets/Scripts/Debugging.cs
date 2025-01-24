
using UnityEngine;

public class Debugging : MonoBehaviour
{
    public bool GUI_on = false;
    private GameManager Gm => GameManager.Instance;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (Gm.Project == null) return;
        Gm.Project.Simulator.DrawGizmos(Color.white);
        if (Gm.Project.SpatialHashGridNodes != null)
        {
            foreach (var cell in Gm.Project.SpatialHashGridNodes.Keys)
            {
                var scalefactor = 0.2f;
                Gizmos.color = new Color((cell.x * scalefactor).Fract(), (cell.y * scalefactor).Fract(), (cell.z*scalefactor).Fract());
                foreach (var node in Gm.Project.SpatialHashGridNodes[cell])
                {
                    Gizmos.DrawSphere(node.Position, 0.2f);
                }
            }
        }
        if (Gm.Hover.HoveredStitch==null) return;
        Gizmos.color = Color.black;
        Gizmos.DrawCube(Gm.Hover.HoveredStitch.Position, new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(Gm.Hover.HoveredNode.Position,0.1f);
    }
    private void OnGUI()
    {
        if (!GUI_on) return;
        foreach (var p in Gm.Project.GetPanels())
        {
            var BBValue = Gm.Hover.IMGUIBoundingBox(p);
            Rect boundingBox = Rect.MinMaxRect(BBValue.Min.x,BBValue.Min.y,BBValue.Max.x,BBValue.Max.y);
            GUI.Box(boundingBox, p.Name+ " bounding box");
        }
    }
    
    
    
}

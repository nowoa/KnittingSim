using UnityEngine;

public class Debugging : MonoBehaviour
{
    public bool GUI_on = false;
    public static Debugging Instance;
    private GameManager Gm => GameManager.Instance;

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        /*DrawSpatialHashGrid();*/
        /*DrawScreenHashGrid();*/
        DrawHoverCheck();
        DrawHoveredStitch();
        
        if(Gm.Project.Simulator!=null) Gm.Project.Simulator.DrawGizmos(Color.white);
        DrawHoveredNode();
        
    }

    private void DrawSpatialHashGrid()
    {
        if (Gm.Project == null) return;
        Gm.Project.Simulator.DrawGizmos(Color.white);
        if (Gm.Project.HashGridWorld != null)
        {
            foreach (var cell in Gm.Project.HashGridWorld.Keys)
            {
                var scalefactor = 0.2f;
                Gizmos.color = new Color((cell.x * scalefactor).Fract(), (cell.y * scalefactor).Fract(), (cell.z*scalefactor).Fract());
                foreach (var index in Gm.Project.HashGridWorld[cell])
                {
                    Gizmos.DrawSphere(Gm.Project.Nodes[index].Position, 0.2f);
                }
            }
        }
    }
    
    private void DrawScreenHashGrid()
    {
        if (Gm.Project == null) return;
        Gm.Project.Simulator.DrawGizmos(Color.white);
        if (Gm.Project.HashGridScreen != null)
        {
            foreach (var cell in Gm.Project.HashGridScreen.Keys)
            {
                var scalefactor = 0.2f;
                Gizmos.color = new Color((cell.x * scalefactor).Fract(), (cell.y * scalefactor).Fract(),0);
                foreach (var index in Gm.Project.HashGridScreen[cell])
                {
                    Vector3 pos = Gm.Project.Stitches[index].Position;
                    Gizmos.DrawSphere(pos, 0.2f);
                }
            }
        }
    }

    private void DrawHoverCheck()
    {
        if (Gm.Project == null) return;
        foreach (var s in Gm.Hover.StitchesInRadius)
        {
            Gizmos.color = new Color(0,1,0,0.3f);
            Gizmos.DrawSphere(s.Position,0.3f);
        }
    }

    private void DrawCollisionRadii()
    {
        if (Gm.Project is null) return;
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        foreach (var node in Gm.Project.Nodes)
        {
            Gizmos.DrawSphere(node.Position, node.CollisionRadius);
        }
    }

    private void DrawHoveredStitch()
    {
        if (Gm.Hover.HoveredStitch==null) return;
        Gizmos.color = Color.black;
        if (Gm.Hover.HoveredStitch!=null)Gizmos.DrawCube(Gm.Hover.HoveredStitch.Position, new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(Gm.Hover.HoveredNode.Position,0.2f);
    }

    private void DrawHoveredNode()
    {
        Gizmos.color = Color.magenta;
        if (Gm.Hover.HoveredNode!=null)Gizmos.DrawSphere(Gm.Hover.HoveredNode.Position, 0.1f);
    }
    
}

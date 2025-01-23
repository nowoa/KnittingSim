using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Debugging : MonoBehaviour
{
    public bool GUI_on = false;
    private GameManager Gm => GameManager.Instance;

    private void OnDrawGizmos()
    {
        if (Gm.Project == null) return;
        Gm.Project.Simulator.DrawGizmos(Color.white);
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

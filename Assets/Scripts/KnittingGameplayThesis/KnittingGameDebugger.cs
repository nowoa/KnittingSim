using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KnittingGameDebugger : MonoBehaviour
{
    KnittingGameManager kgm => KnittingGameManager.Instance;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        /*DrawNodes();*/
        DrawText();
    }

    private void DrawNodes()
    {
        foreach (var n in kgm.NodesToSimulate)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(n.Position,0.1f);
            
        }
    }

    private void DrawText()
    {
        foreach (var n in kgm.NodesToSimulate)
        {
            Handles.Label(n.Position, n.id.ToString());
        }
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnittingGameDebugger : MonoBehaviour
{
    KnittingGameManager kgm => KnittingGameManager.Instance;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        DrawNodes();
    }

    private void DrawNodes()
    {
        foreach (var n in kgm.nodes)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(n.Position,0.1f);
        }
    }
}

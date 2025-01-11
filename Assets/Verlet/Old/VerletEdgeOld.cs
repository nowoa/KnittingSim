/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Verlet
{
    
    
    

    public class VerletEdgeOld
    {
        public enum EdgeType
        {
            Structural,
            Shear,
            Bend,
            Seam
        }

        public EdgeType edgeType;
        
        public float Length => length;

        public VerletNodeOld a, b;
        private float length;

        public VerletEdge(VerletNodeOld a, VerletNodeOld b)
        {
            this.a = a;
            this.b = b;
            this.length = (a.Position - b.Position).magnitude;

        }

        public VerletEdge(VerletNodeOld a, VerletNodeOld b, float len, EdgeType type)
        {
            this.a = a;
            this.b = b;
            this.length = len;
            edgeType = type;

        }

        public VerletNodeOld Other(VerletNodeOld p)
        {
            if (a == p)
            {
                return b;
            }
            else
            {
                return a;
            }
        }

        public static void ConnectNodes(VerletNodeOld a, VerletNodeOld b)
        {
            VerletEdge edge = new VerletEdge(a, b);
            a.AddEdge(edge);
            b.AddEdge(edge);
        }

        public static void ConnectNodes(VerletNodeOld a, VerletNodeOld b, float length, EdgeType type)
        {
            if (a.FindEdgeByNode(b) != null)
            {
                Debug.LogWarning("there is already an edge between these nodes!");
                return;
            }
            VerletEdge edge = new VerletEdge(a, b, length, type);
            a.AddEdge(edge);
            b.AddEdge(edge);
        }
    }
}*/
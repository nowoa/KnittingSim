using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Verlet
{

    public class VerletEdge
    {
        private float _length;
        public float Length => _length;
        public VerletNode a, b;
        public enum EdgeType
        {
            Structural,
            Shear,
            Bend,
            Seam
        }

        public EdgeType edgeType;
        
        public VerletEdge(VerletNode a, VerletNode b, float len, EdgeType type)
        {
            this.a = a;
            this.b = b;
            _length = len;
            edgeType = type;

        }

        public VerletNode Other(VerletNode p)
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
        
        public static void ConnectNodes(VerletNode a, VerletNode b, float length, EdgeType type)
        {
            if (a.GetEdgeByNode(b) != null)
            {
                Debug.LogWarning("there is already an edge between these nodes!");
                return;
            }
            VerletEdge edge = new VerletEdge(a, b, length, type);
            a.AddEdge(edge);
            b.AddEdge(edge);
        }

        public void RemoveEdge()
        {
            a.Connection.Remove(this);
            b.Connection.Remove(this);
            //make sure there are no connections to this edge anywhere (e.g. being referenced by the structural edge variable on a node or something)
        }
        
    }
}
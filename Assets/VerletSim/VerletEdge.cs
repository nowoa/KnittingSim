using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Verlet
{


    public class VerletEdge
    {
        public float Length => length;

        private VerletNode a, b;
        private float length;

        public VerletEdge(VerletNode a, VerletNode b)
        {
            this.a = a;
            this.b = b;
            this.length = (a.position - b.position).magnitude;

        }

        public VerletEdge(VerletNode a, VerletNode b, float len)
        {
            this.a = a;
            this.b = b;
            this.length = len;

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

        public static void ConnectNodes(VerletNode a, VerletNode b)
        {
            VerletEdge edge = new VerletEdge(a, b);
            a.AddEdge(edge);
            b.AddEdge(edge);
        }

    }
}
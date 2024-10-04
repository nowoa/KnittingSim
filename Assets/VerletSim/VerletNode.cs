using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Verlet
{

    public class VerletNode
    {
        public List<VerletEdge> Connection => connection;

        public Vector3 position;
        public Vector3 initPos;
        protected Vector3 prev;
        public bool isAnchored;
        private List<VerletEdge> connection;

        public VerletNode(Vector3 p)
        {
            position = prev = p;
            connection = new List<VerletEdge>();
        }

        public void Step()
        {
            var v = position - prev;
            var next = position + (v*0.9f);
            prev = position;
            position = next;
        }
        
        public void AddEdge(VerletEdge e)
        {
            connection.Add(e);
        }
        
        
    }
}
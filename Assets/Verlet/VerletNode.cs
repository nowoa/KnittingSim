using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Verlet
{
    public class VerletNode
    {
        #region Simulation

        public Vector3 Position;
        public int id;
        private Vector3 Prev;
        private List<VerletEdge> _connection;
        public List<VerletEdge> Connection => _connection;
        
        public VerletNode(Vector3 p)
        {
            Position = Prev = p;
            _connection = new List<VerletEdge>();
        }

        public void Step()
        {
            var v = Position - Prev;
            var next = Position + (v*0.9f);
            Prev = Position;
            Position = next;
        }
        
        public void AddEdge(VerletEdge e)
        {
            _connection.Add(e);
        }

        #endregion

        public Stitch ParentStitch;
        public Panel ParentPanel { get; private set; }
        public VerletNode[] Neighbors { get; private set; } = new VerletNode[4];
        public Vector2 Dimensions { get; private set; }
        public float CollisionRadius;
        public Vector3 Normal { get; private set; }

        public VerletNode[] DirectNeighbors { get; private set; } = Array.Empty<VerletNode>();

        public enum Neighbor
        {
            up, 
            right,
            down,
            left
        }

        public void SetNeighborNode(Neighbor myNeighbor, VerletNode myNode)
        {
            if (Neighbors[(int)myNeighbor] != null)
            {
                Debug.LogWarning("neighbor is already assigned.");
                return;
            }

            Neighbors[(int)myNeighbor] = myNode;
        }

        public void SetParentPanel(Panel myParent)
        {
            ParentPanel = myParent;
        }

        public VerletNode GetNeighbor(Neighbor myNeighbor)
        {
            return Neighbors[(int)myNeighbor];
        }
        

        public void RemoveNode()
        {
            
        }

        public List<VerletEdge> GetEdgesOfType(VerletEdge.EdgeType myType)
        {
            return null;
        }

        public VerletEdge GetEdgeByNode(VerletNode other)
        {
            foreach (var e in _connection)
            {
                if (e.Other(this) == other) return e;
            }

            return null;
        }

        public void SetSize(float myWidth, float myHeight)
        {
            Dimensions = new Vector2(myWidth, myHeight);
        }

        public void SetCollisionRadius()
        {
            var sizeFactor = 1f;
            var minValue = _connection.Select(item => item.Length).Min();
            CollisionRadius = minValue * sizeFactor;
        }
        
        public void UpdateNormal()
        {
            var nodeLeft = this.Traverse(Neighbor.left);
            var nodeDown = this.Traverse(Neighbor.down);
            var nodeDownLeft = this.Traverse(Neighbor.left)?.Traverse(Neighbor.down);
            int normalCount = 0;
            Vector3 averageNormal = new Vector3();

            if (ParentStitch != null)
            {
                averageNormal += ParentStitch.Normal;
                normalCount++;
            }

            if (nodeLeft != null && nodeLeft.ParentStitch != null)
            {
                averageNormal += nodeLeft.ParentStitch.Normal;
                normalCount++;
            }

            if (nodeDown != null && nodeDown.ParentStitch != null)
            {
                averageNormal += nodeDown.ParentStitch.Normal;
                normalCount++;
            }

            if (nodeDownLeft != null && nodeDownLeft.ParentStitch != null)
            {
                averageNormal += nodeDownLeft.ParentStitch.Normal;
                normalCount++;
            }
            
            if (normalCount == 0)
            {
                Debug.LogWarning("no normals! setting to vector3.zero");
                Normal = Vector3.zero;
                return;
            }

            Normal = (averageNormal / normalCount).normalized;
        }

        public void ChangeEdgeLength(VerletNode myTarget, float myLength)
        {
            var edgeToChange = GetEdgeByNode(myTarget);
            var edgetype = edgeToChange.edgeType;
            myTarget._connection.Remove(edgeToChange);
            _connection.Remove(edgeToChange);
            VerletEdge.ConnectNodes(this, myTarget, myLength, edgetype);
        }

        public void GetDirectNeighbors()
        {
            DirectNeighbors = _connection.Where(item => item.edgeType != VerletEdge.EdgeType.Bend).Select(item => item.Other(this)).ToArray();
        }
    }

    public class AnchoredNode
    {
        public VerletNode Node;
        public Vector3 AnchoredPos;
        public GameObject PinNeedle;
    }
}
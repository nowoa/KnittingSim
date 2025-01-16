using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        
        private Stitch _parentStitch;
        public Panel ParentPanel { get; private set; }
        private VerletNode[] _neighbors = new VerletNode[4];
        public bool IsAnchored { get; private set; }
        public Vector3 AnchoredPosition;

        public enum Neighbor
        {
            up, 
            right,
            down,
            left
        }

        public void SetNeighborNode(Neighbor myNeighbor, VerletNode myNode)
        {
            if (_neighbors[(int)myNeighbor] != null)
            {
                Debug.LogWarning("neighbor is already assigned.");
                return;
            }

            _neighbors[(int)myNeighbor] = myNode;
        }

        public void SetParentPanel(Panel myParent)
        {
            ParentPanel = myParent;
        }

        public VerletNode GetNeighbor(Neighbor myNeighbor)
        {
            return _neighbors[(int)myNeighbor];
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
            return null;
        }

        public void ToggleAnchored(Vector3 anchorPos)
        {
            IsAnchored = !IsAnchored;
            if (IsAnchored)
            {
                ParentPanel.AnchoredNodes.Add(this);
            }
            else
            {
                ParentPanel.AnchoredNodes.Remove(this);
            }
            AnchoredPosition = anchorPos;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace Verlet
{

    public class VerletNode
    {
        #region private variables
        
        private List<VerletEdge> _connection;
        private List<VerletNode> _nodesAbove;
        private List<VerletNode> _nodesBelow;
        private VerletNode _nodeLeft;
        private VerletNode _nodeRight;
        private VerletEdge _bendEdgeHorizontal;
        private VerletEdge _bendEdgeVertical;
        private Vector3 Prev;

        #endregion

        #region access givers

        public List<VerletEdge> Connection => _connection;
        public List<VerletNode> NodesAbove => _nodesAbove;
        public List<VerletNode> NodesBelow => _nodesBelow;
        public VerletNode NodeLeft => _nodeLeft;
        public VerletNode NodeRight => _nodeRight;
        public VerletEdge BendEdgeHorizontal => _bendEdgeHorizontal;
        public VerletEdge BendEdgeVertical => _bendEdgeVertical;

        #endregion
        
        public Vector3 Position;
        public bool IsAnchored;
        public Vector3 AnchoredPos;

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

        public void AddNodeAbove(VerletNode n)
        {
            if (_nodesAbove == null)
            {
                _nodesAbove = new List<VerletNode>();
                
            }
            _nodesAbove.Add(n);
        }

        public void AddNodeBelow(VerletNode n)
        {
            if (_nodesBelow == null)
            {
                _nodesBelow = new List<VerletNode>();
                
            }
            _nodesBelow.Add(n);
        }

        public void SetNodeLeft(VerletNode n)
        {
            _nodeLeft = n;
        }

        public void SetNodeRight(VerletNode n)
        {
            _nodeRight = n;
        }

        public void SetHorizontalBendEdge(VerletEdge e)
        {
            _bendEdgeHorizontal = e;
        }

        public void SetVerticalBendEdge(VerletEdge e)
        {
            _bendEdgeVertical = e;
        }
        
        public void RemoveAllEdges()
        {
            foreach (var edge in new List<VerletEdge>(_connection))
            {
                edge.Other(this).Connection.Remove(edge);
                _connection.Remove(edge);
            }
        }

        public void RemoveBendEdge(bool horizontal)
        {
            if (horizontal)
            {
                _bendEdgeHorizontal.Other(this).Connection.Remove(_bendEdgeHorizontal);
                _connection.Remove(_bendEdgeHorizontal);
            }
            else
            {
                _bendEdgeVertical.Other(this).Connection.Remove(_bendEdgeVertical);
                _connection.Remove(_bendEdgeVertical);
            }
        }
    }
}
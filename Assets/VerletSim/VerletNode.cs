using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace Verlet
{
//TO DO: restructure adding edges and removing edges functions into one big function
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
        private VerletEdge _shearEdgeUp;
        private VerletEdge _shearEdgeDown;
        private VerletEdge _edgeUp;
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
        public VerletEdge ShearEdgeUp => _shearEdgeUp;
        public VerletEdge ShearEdgeDown => _shearEdgeDown;
        public VerletEdge EdgeUp => _edgeUp;

        #endregion
        
        public Vector3 Position;
        public bool IsAnchored;
        public Vector3 AnchoredPos;

        public VerletNode(Vector3 p)
        {
            Position = Prev = p;
            _connection = new List<VerletEdge>();
            _nodesAbove = new List<VerletNode>();
            _nodesBelow = new List<VerletNode>();
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
            _nodesAbove.Add(n);
        }

        public void AddNodeBelow(VerletNode n)
        {
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

        public void SetShearEdge(VerletEdge e, bool up = true)
        {
            if (up) _shearEdgeUp = e;
            else
            {
                _shearEdgeDown = e;
            }

        }

        public void SetVerticalEdge(VerletEdge e)
        {
            _edgeUp = e;
        }

        public void RemoveShearEdge(bool up = true)
        {
            if (up)
            {
                if (_shearEdgeUp != null)
                {
                    _shearEdgeUp.Other(this).Connection.Remove(_shearEdgeUp);
                    _connection.Remove(_shearEdgeUp);
                }
            }
            else
            {
                if (_shearEdgeDown != null)
                {
                    _shearEdgeDown.Other(this).Connection.Remove(_shearEdgeDown);
                    _connection.Remove(_shearEdgeDown);
                }
            }
        }

        public void RemoveVerticalEdge()
        {
            _edgeUp.Other(this).Connection.Remove(_edgeUp);
            _connection.Remove(_edgeUp);
        }

    }
}
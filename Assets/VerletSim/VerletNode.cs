using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Verlet.VerletNode.EdgeType;


namespace Verlet
{
    public class VerletNode
    {
        #region private variables
        
        private List<VerletEdge> _connection;
        private VerletNode _nodeAbove;
        private VerletNode _nodeBelow;
        private VerletNode _nodeLeft;
        private VerletNode _nodeRight;
        private VerletEdge _bendEdgeHorizontal;
        private VerletEdge _bendEdgeVertical;
        private VerletEdge _shearEdgeUp;
        private VerletEdge _shearEdgeDown;
        private VerletEdge _edgeUp;
        private VerletEdge _edgeRight;
        private StitchInfo _parent;
        private Vector3 Prev;
        

        #endregion

        #region access givers

        public List<VerletEdge> Connection => _connection;
        public VerletNode NodeAbove => _nodeAbove;
        public VerletNode NodeBelow => _nodeBelow;
        public VerletNode NodeLeft => _nodeLeft;
        public VerletNode NodeRight => _nodeRight;
        public VerletEdge BendEdgeHorizontal => _bendEdgeHorizontal;
        public VerletEdge BendEdgeVertical => _bendEdgeVertical;
        public VerletEdge ShearEdgeUp => _shearEdgeUp;
        public VerletEdge ShearEdgeDown => _shearEdgeDown;
        public VerletEdge EdgeUp => _edgeUp;
        public VerletEdge EdgeRight => _edgeRight;
        public StitchInfo Parent => _parent;
        public enum EdgeType
        {
            Structural,
            Bend,
            Shear
        }

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

        public void SetNodeAbove(VerletNode n)
        {
            _nodeAbove = n;
        }

        public void SetNodeBelow(VerletNode n)
        {
            _nodeBelow = n;
        }

        public void SetNodeLeft(VerletNode n)
        {
            _nodeLeft = n;
        }

        public void SetNodeRight(VerletNode n)
        {
            _nodeRight = n;
        }

        public void SetParentStitch(StitchInfo parent)
        {
            _parent = parent;
        }

        public void SetBendEdge(VerletEdge edge, bool up)
        {
            if (up)
                _bendEdgeVertical = edge;
            else
                _bendEdgeHorizontal = edge;
        }

        public void SetShearEdge(VerletEdge edge, bool up)
        {
            if (up)
                _shearEdgeUp = edge;
            else
                _shearEdgeDown = edge;
        }

        public void SetStructuralEdge(VerletEdge edge, bool up)
        {
            if (up)
                _edgeUp = edge;
            else
                _edgeRight = edge;
        }

        public void RemoveBendEdge(bool up)
        {
            var edgeToRemove = up ? _bendEdgeVertical : _bendEdgeHorizontal;
            if (edgeToRemove != null)
            {
                edgeToRemove.Other(this).Connection.Remove(edgeToRemove);
                _connection.Remove(edgeToRemove);
                if (up) _bendEdgeVertical = null;
                else _bendEdgeHorizontal = null;
            }
        }

        public void RemoveShearEdge(bool up)
        {
            var edgeToRemove = up ? _shearEdgeUp : _shearEdgeDown;
            if (edgeToRemove != null)
            {
                edgeToRemove.Other(this).Connection.Remove(edgeToRemove);
                _connection.Remove(edgeToRemove);
                if (up) _shearEdgeUp = null;
                else _shearEdgeDown = null;
            }
        }

        public void RemoveStructuralEdge(bool up)
        {
            var edgeToRemove = up ? _edgeUp : _edgeRight;
            if (edgeToRemove != null)
            {
                edgeToRemove.Other(this).Connection.Remove(edgeToRemove);
                _connection.Remove(edgeToRemove);
                if (up) _edgeUp = null;
                else _edgeRight = null;
            }
        }
        public void RemoveAllEdges()
        {
            foreach (var edge in new List<VerletEdge>(_connection))
            {
                edge.Other(this).Connection.Remove(edge);
                _connection.Remove(edge);
            }
        }
    }
}
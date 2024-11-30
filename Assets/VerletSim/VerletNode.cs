using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Verlet
{
    public class VerletNode
    {
        #region private variables
        
        private List<VerletEdge> _connection;
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
        public VerletEdge BendEdgeHorizontal => _bendEdgeHorizontal;
        public VerletEdge BendEdgeVertical => _bendEdgeVertical;
        public VerletEdge ShearEdgeUp => _shearEdgeUp;
        public VerletEdge ShearEdgeDown => _shearEdgeDown;
        public VerletEdge EdgeUp => _edgeUp;
        public VerletEdge EdgeRight => _edgeRight;
        public StitchInfo Parent => _parent;
        public Vector3 normal;
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

        public void SetParentStitch(StitchInfo parent)
        {
            _parent = parent;
        }

        public void SetBendEdge(bool up,VerletEdge edge = null)
        {
            if (edge == null)
            {
                edge = _connection.Last();
            }

            if (up)
            {
                if (_bendEdgeVertical != null)
                {
                    Debug.LogWarning(
                        "bend edge vertical was overwritten without removing the previous edge. this can cause duplicate edges");
                }
                _bendEdgeVertical = edge;
            }

            else
            {
                if (_bendEdgeHorizontal != null)
                {
                    Debug.LogWarning(
                        "bend edge horizontal was overwritten without removing the previous edge. this can cause duplicate edges");
                }
                _bendEdgeHorizontal = edge;
            }
                
        }

        public void SetShearEdge(bool up, VerletEdge edge = null)
        {
            if (edge == null)
            {
                edge = _connection.Last();
            }
            if (up)
            {
                if (_shearEdgeUp != null)
                {
                    Debug.LogWarning(
                        "shear edge up was overwritten without removing the previous edge. this can cause duplicate edges");
                }

                _shearEdgeUp = edge;
            }
            else
            {
                if (_shearEdgeDown != null)
                {
                    Debug.LogWarning(
                        "shear edge down was overwritten without removing the previous edge. this can cause duplicate edges");
                }
                _shearEdgeDown = edge;
            }
                
        }

        public void SetStructuralEdge(bool up, VerletEdge edge = null)
        {
            if (edge == null)
            {
                edge = _connection.Last();
            }

            if (up)
            {
                if (_edgeUp != null)
                {
                    Debug.LogWarning(
                        "structural edge up was overwritten without removing the previous edge. this can cause duplicate edges");
                }
                _edgeUp = edge;
            }

            else
            {
                if (_edgeRight != null)
                {
                    Debug.LogWarning(
                        "structural edge right was overwritten without removing the previous edge. this can cause duplicate edges");
                }
                _edgeRight = edge;
            }
                
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

            _bendEdgeHorizontal = null;
            _bendEdgeVertical = null;
            _shearEdgeDown = null;
            _shearEdgeUp = null;
            _edgeUp = null;
            _edgeRight = null;
        }

        public VerletEdge FindEdgeByNode(VerletNode other)
        {
            VerletEdge returnEdge = null;
            foreach (var edge in _connection)
            {
                if (edge.a == this && edge.b == other)
                {
                    returnEdge = edge;
                }
                else if (edge.a == other && edge.b == this)
                {
                    returnEdge = edge;
                }
            }
            if (returnEdge == null) Debug.Log("no edge between those two nodes!");
            return returnEdge;
        }

        public static void RemoveEdge(VerletEdge edge)
        {
            if (edge.a != null && edge.b != null)
            {
                edge.a.Connection.Remove(edge);
                edge.b.Connection.Remove(edge);
            }
        }

        private List<VerletEdge> GetEdgesOfType(List<VerletEdge> edges, VerletEdge.EdgeType targetType)
        {
            return edges.Where(edge => edge.edgeType == targetType).ToList();
        }
        public void CalculateNormal()
        {
            VerletEdge edge1 = null;
            VerletEdge edge2 = null;
            bool upReverse = false;
            bool rightReverse = false;

            if (_edgeUp != null)
            {
                edge1 = _edgeUp;
            }

            if (_edgeRight != null)
            {
                edge2 = _edgeRight;
            }

            if (_edgeUp == null)
            {
                var edge = GetEdgesOfType(_connection, VerletEdge.EdgeType.Structural);
                foreach (var e in edge)
                {
                    if (e.Other(this).EdgeUp == e)
                    {
                        edge1 = e;
                        break;
                    }
                }

                upReverse = true;
            }

            if (_edgeRight == null)
            {
                var edge = GetEdgesOfType(_connection, VerletEdge.EdgeType.Structural);
                foreach (var e in edge)
                {
                    if (e.Other(this).EdgeRight == e)
                    {
                        edge2 = e;
                        break;
                    }
                }

                rightReverse = true;
            }
            Vector3 up = edge1 == null? Position :  upReverse? Position - edge1.Other(this).Position : edge1.Other(this).Position - Position;
            Vector3 right = edge2 == null? Position : rightReverse? Position - edge2.Other(this).Position : edge2.Other(this).Position - Position;
            
            normal = Vector3.Cross(up,right).normalized;

        }
    }
}
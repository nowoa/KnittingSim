using System.Collections.Generic;
using UnityEngine;

namespace Verlet
{
    public class VerletSimulator
    {
        private List<VerletNode> _nodes;
        public List<VerletNode> Nodes => _nodes;
        private Vector3 _gravity = new Vector3(0, GameManager.GravityFactor, 0);

        public VerletSimulator(List<VerletNode> nodes)
        {
            _nodes = nodes;
        }

        public void Simulate(int iterations, float dt)
        {
            Step(dt);
            for (int iter = 0; iter < iterations; iter++)
            {
                Solve();
            }
            SolveSelfCollisionExpensive();
        }

        void Step(float deltaTime)
        {
            _nodes.ForEach(p =>
            {
                p.Position += _gravity * deltaTime;
                p.Step();
            });
        }

        void Solve()
        {
            _nodes.ForEach(p => Solve(p));
        }

        void Solve(VerletNode particle)
        {
            if (particle.IsAnchored) return;
            particle.Connection.ForEach(e =>
            {
                var other = e.Other(particle);
                Solve(particle, other, e.Length);
            });
        }

        void Solve(VerletNode a, VerletNode b, float rest)
        {
            var delta = a.Position - b.Position;
            var current = delta.magnitude;
            var buffer = rest * 0.4f;
            if (Mathf.Abs(current - rest) <= buffer)
            {
                return;
            }

            if (current >= rest + buffer)
            {
                rest = rest + buffer;
            }

            if (current <= rest - buffer)
            {
                rest = rest - buffer;
            }

            var f = (current - rest) / current;
            a.Position -= f * 0.5f * delta;
            b.Position += f * 0.5f * delta;
        }

        void SolveSelfCollisionExpensive()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                for (int j = i + 1; j < _nodes.Count; j++) // Avoid redundant checks
                {
                    var nodeA = _nodes[i];
                    var nodeB = _nodes[j];
                    /*if (nodeA.isSeam || nodeB.isSeam)
                    {
                        continue;
                    }*/

                    if (nodeA.Connection.Count > 12 || (nodeA.Connection.Count < 12 && nodeA.Connection.Count>= 5))
                    {
                        continue; //make sure it isnt trying to push apart decreases
                    }

                    if (nodeA.ParentStitch?.Corners[1] == nodeB ||
                        nodeA.GetNeighbor(VerletNode.Neighbor.right)== nodeB ||
                        nodeA.ParentStitch?.Corners[2] == nodeB ||
                        nodeA.ParentStitch?.Corners[3].GetNeighbor(VerletNode.Neighbor.down) == nodeB)
                    {
                        
                        continue;
                    }
                    
                    // Calculate the distance between the nodes
                    var delta = nodeA.Position - nodeB.Position;
                    var distance = delta.magnitude;
                    var minDistance = nodeA.CollisionRadius + nodeB.CollisionRadius;

                    if (distance < minDistance)
                    {
                        // Calculate the amount to push outward
                        float difference = minDistance - distance;

                        // Normalize the delta vector to get the separation direction
                        Vector3 direction = delta.normalized;

                        // Push both nodes outward equally
                        nodeA.Position += 0.5f * difference * direction;
                        nodeB.Position -= 0.5f * difference * direction;
                    }
                }
            }
        }

        public void DrawGizmos(Color myColor)
            {
                for (int i = 0, n = _nodes.Count; i < n; i++)
                {
                    var p = _nodes[i];
                    Gizmos.color = myColor;
                    p.Connection.ForEach(e =>
                    {
                        var other = e.Other(p);
                        Gizmos.DrawLine(p.Position, other.Position);
                    });
                    Gizmos.DrawSphere(p.Position,0.1f);
                }
            }
    }
}
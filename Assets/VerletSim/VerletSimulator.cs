using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verlet
{
    public class VerletSimulator
    {
        private List<VerletNode> particles;
        public List<VerletNode> Nodes => particles;
        private Vector3 _gravity = new Vector3(0, -0.1f, 0);

        public VerletSimulator(List<VerletNode> particles)
        {
            this.particles = particles;
        }

        public void Simulate(int iterations, float dt)
        {
            var time = dt / iterations;
            Step(dt);
            for (int iter = 0; iter < iterations; iter++)
            {
                Solve();
                SolveSelfCollision();
            }
        }

        void Step(float deltaTime)
        {
            particles.ForEach(p =>
            {
                p.Position += _gravity * deltaTime;
                p.Step();
            });
        }

        void Solve()
        {
            particles.ForEach(p => Solve(p));
        }

        void Solve(VerletNode particle)
        {
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

        void SolveSelfCollision()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                for (int j = i + 1; j < particles.Count; j++) // Avoid redundant checks
                {
                    var nodeA = particles[i];
                    var nodeB = particles[j];

                    /*if (nodeA.Connection.Select(item => item.Other(nodeA)).Contains(nodeB))
                    {
                        return;
                    }*/

                    // Calculate the distance between the nodes
                    var delta = nodeA.Position - nodeB.Position;
                    var distance = delta.magnitude;
                    var minDistance = nodeA.MarbleRadius + nodeB.MarbleRadius;

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
                for (int i = 0, n = particles.Count; i < n; i++)
                {
                    var p = particles[i];
                    Gizmos.color = myColor;
                    p.Connection.ForEach(e =>
                    {
                        var other = e.Other(p);
                        Gizmos.DrawLine(p.Position, other.Position);
                    });
                }
            }
    }
}
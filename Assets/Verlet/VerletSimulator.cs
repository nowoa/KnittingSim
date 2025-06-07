using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verlet
{
    public class VerletSimulator
    {
        private List<VerletNode>_nodes;
        public List<VerletNode> Nodes => _nodes;
        private Vector3 _gravity = new Vector3(0, GameManager.GravityFactor, 0);

        public VerletSimulator(IList<VerletNode> nodes)
        {
            _nodes = nodes.ToList();
        }

        public void Simulate(int iterations, float dt)
        {
            Step(dt);
            for (int iter = 0; iter < iterations; iter++)
            {
                Solve();
            }
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
            /*if (GameManager.Instance.Project.anchors.GetAnchors().ContainsKey(particle)) return;*/
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
                    /*if (GameManager.Instance.Project.anchors.GetAnchors().ContainsKey(p)) Gizmos.color = Color.cyan;*/
                    /*Gizmos.DrawSphere(p.Position,0.1f);*/
                }
            }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Verlet
{
    public class VerletSimulator
    {
        private List<VerletNode> particles;
        public List<VerletNode> Nodes => particles;
        private Vector3 _gravity = new Vector3(0, -1f, 0);

        public VerletSimulator(List<VerletNode> particles)
        {
            this.particles = particles;
        }
        
        public void Simulate(int iterations, float dt)
        {
            var time = dt / iterations;
            for (int iter = 0; iter < iterations; iter++ )
            {
                Step(time);
                Solve();
            }
        }
        
        void Step(float deltaTime)
        {
            
            particles.ForEach(p =>
            {
                p.position += _gravity*deltaTime;
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
                Solve(particle,other,e.Length);
            });
        }
        
        void Solve(VerletNode a, VerletNode b, float rest)
        {
            var delta = a.position - b.position;
            var current = delta.magnitude;
            var buffer = rest * 0.3f;
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
            a.position -= f * 0.5f * delta;
            b.position += f * 0.5f * delta;


            /*var delta = a.position - b.position;
            var direction = delta.normalized;
            var currentLength = delta.magnitude;
            var error = currentLength - rest;
            a.position -= error * 0.5f * direction;
            b.position += error * 0.5f * direction;*/

        }
        
        
        public void DrawGizmos()
        {
            for(int i =0, n = particles.Count; i<n;i++)
            {
                var p = particles[i];/*
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(p.position,0.2f);*/
                
                Gizmos.color=Color.white;
                p.Connection.ForEach(e =>
                {
                    var other = e.Other(p);
                    Gizmos.DrawLine(p.position,other.position);
                });
            }
        }
        
    }
}
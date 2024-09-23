using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verlet
{
    public class ExampleSimulation : MonoBehaviour
    {
        private VerletSimulator _sim;

        private void OnEnable()
        {
            VerletNode[] nodes = new VerletNode[500];
            float distance = 1f;
            int columns = 20;
            int rows = nodes.Length/columns;
            for (int i = 0; i < nodes.Length; i++)
            {
                
                nodes[i] = new VerletNode(transform.position + new Vector3(i%columns*distance, Mathf.Floor(i/columns)*distance,0));
                

                if (i-columns >= 0)
                {
                    VerletEdge.ConnectNodes(nodes[i],nodes[i-columns]);
                }

                /*if (i + rows <= nodes.Length)
                {
                    VerletEdge.ConnectNodes(nodes[i],nodes[i+rows]);
                }

                if ((i + 1) % columns != 0 && (i+1)<=nodes.Length)
                {
                    VerletEdge.ConnectNodes(nodes[i],nodes[i+1]);
                }*/

                if (i % columns != 0 && (i-1)>=0)
                {
                    VerletEdge.ConnectNodes(nodes[i],nodes[i-1]);
                }
                
            }
            _sim = new VerletSimulator(nodes.ToList());
        }

        private void FixedUpdate()
        {
            _sim.Simulate(10, Time.fixedDeltaTime);
            _sim.Nodes[0].position = transform.position;
        }

        private void OnDrawGizmos()
        {
            if (_sim != null)
            {
                _sim.DrawGizmos();
            }
            
        }
    }
}

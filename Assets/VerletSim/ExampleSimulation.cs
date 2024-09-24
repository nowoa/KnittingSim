using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Verlet
{
    public class ExampleSimulation : MonoBehaviour
    {
        private VerletNode[] nodes;
        private VerletSimulator _sim;
        private Vector3 _startingPosition;
        private void OnEnable()
        {
             nodes = new VerletNode[100];
            float distance = 1f;
            int columns = 10;
            int rows = nodes.Length/columns;
            for (int i = 0; i < nodes.Length; i++)
            {
                
                nodes[i] = new VerletNode(transform.position + new Vector3(i%columns*distance, Mathf.Floor(i/columns)*distance,0));
                nodes[i].initPos = nodes[i].position;
                

                if (i-columns >= 0) // connect node above
                {
                    VerletEdge.ConnectNodes(nodes[i],nodes[i-columns]);
                }
                if (i % columns != 0 && (i-1)>=0) // connect node to the left
                {
                    VerletEdge.ConnectNodes(nodes[i],nodes[i-1]);
                }
                if(i-columns-1>=0 && i % columns != 0) // connect node diagonal left
                {
                    VerletEdge.ConnectNodes(nodes[i],nodes[i-columns-1]);
                }                   
                    if(i-columns+1>=0 && (i+1)%columns!=0)
                {
                    /*connect node diagonal right*/
                    VerletEdge.ConnectNodes(nodes[i],nodes[i-columns+1]);
                }
                
                if(i-2 >=0 && i % columns != 0) // connect node 2 to the left
                {
                    VerletEdge.ConnectNodes(nodes[i], nodes[i-2]);
                }
                if(i-columns*2 >=0) // connect node 2 up
                    {
                       VerletEdge.ConnectNodes(nodes[i], nodes[i-columns*2]);
                    }
                
            }
            _sim = new VerletSimulator(nodes.ToList());

            AnchorNodes();
        }

        void AnchorNodes()
        {
           
        }

        private void FixedUpdate()
        {
            
            /*for (int i = 0;i<10;i++)
            {
                _sim.Nodes[i].position = _sim.Nodes[i].initPos;
            }

            for (int i = nodes.Length - 10; i < nodes.Length; i++)
            {
                _sim.Nodes[i].position = _sim.Nodes[i].initPos;
            }*/
            
            _sim.Nodes[45].position = transform.position;
            _sim.Simulate(10, Time.fixedDeltaTime);
            /*for (int i = 0;i<10;i++)
            {
                _sim.Nodes[i].position = _sim.Nodes[i].initPos;
            }

            for (int i = nodes.Length - 10; i < nodes.Length; i++)
            {
                _sim.Nodes[i].position = _sim.Nodes[i].initPos;
            }
            _sim.Simulate(10, Time.fixedDeltaTime);*/
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

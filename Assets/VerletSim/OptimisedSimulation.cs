using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Verlet
{
    public class OptimisedSimulation : MonoBehaviour
    {
        private VerletNode[] nodes;
        private VerletSimulator _sim;
        private VerletSimulator _simConnected;
        private VerletSimulator _simAnchored;
        private Vector3 _startingPosition;
        private List<VerletNode> _anchoredNodes;
        public int heldStitchIndex;
        private void OnEnable()
        {
            nodes = new VerletNode[4000];
            float distance = 1f;
            int columns = 100;
            int rows = nodes.Length/columns;
            for (int i = 0; i < nodes.Length; i++)
            {
                
                nodes[i] = new VerletNode(transform.position + new Vector3(i%columns*distance, Mathf.Floor(i/columns)*distance,0));
                
                

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
                
                /*if(i-2 >=0 && i % columns != 0 && (i-1)%columns!=0) // connect node 2 to the left
                {
                    VerletEdge.ConnectNodes(nodes[i], nodes[i-2]);
                }
                if(i-columns*2 >=0) // connect node 2 up
                {
                    VerletEdge.ConnectNodes(nodes[i], nodes[i-columns*2]);
                }*/
                _sim = new VerletSimulator(nodes.ToList());
            }

            _anchoredNodes = new List<VerletNode>();
            _anchoredNodes.AddRange(nodes[..columns]);
            _anchoredNodes.AddRange(nodes[^columns..nodes.Length]);
            Debug.Log(_anchoredNodes.Count);
            _simAnchored = new VerletSimulator(_anchoredNodes);
            Debug.Log("anchored nodes:" + _anchoredNodes.Count);
        }

        [ContextMenu("update simulated nodes")]
        private void NodesToSimulate()
        {
            var centralNode = nodes[heldStitchIndex];
            List<VerletNode> connectedNodesLayer1 = new List<VerletNode>();
            List<VerletNode> connectedNodesLayer2 = new List<VerletNode>();
            List<VerletNode> connectedNodesLayer3 = new List<VerletNode>();
            foreach (var edge in centralNode.Connection)
            {
                if (!connectedNodesLayer1.Contains(edge.Other(centralNode)))
                {
                    connectedNodesLayer1.Add(edge.Other(centralNode));
                }
                
            }
            connectedNodesLayer2.AddRange(connectedNodesLayer1);
            foreach (var node in connectedNodesLayer1)
            {
                foreach (var edge in node.Connection)
                {
                    if (!connectedNodesLayer2.Contains(edge.Other(node)))
                    {
                        connectedNodesLayer2.Add(edge.Other(node));
                    }
                }
            }
            foreach (var node in connectedNodesLayer2)
            {
                foreach (var edge in node.Connection)
                {
                    if (!connectedNodesLayer3.Contains(edge.Other(node)))
                    {
                        connectedNodesLayer3.Add(edge.Other(node));
                    }
                }
            }

            
            Debug.Log(connectedNodesLayer3.Count);
            _simConnected = new VerletSimulator(connectedNodesLayer3);
            
        }

        private void FixedUpdate()
        {
            _sim.Simulate(1,Time.fixedDeltaTime);
            _simConnected.Nodes[_simConnected.Nodes.Count/2].Position = transform.position;
            _simConnected.Simulate(5,Time.fixedDeltaTime);
        }

        private void OnDrawGizmos()
        {
            if (_sim != null)
            {
                _sim.DrawGizmos(Color.white);
            }
            
        }
    }
}

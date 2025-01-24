using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace Verlet
{
    public class SelfCollision
    {
        public static bool checkDouble = true;
        public static void Collide(Dictionary<Vector3Int,List<VerletNode>> myHashGrid)
        {
            Dictionary<Vector3Int, List<Vector3Int>> alreadyCheckedCells = new Dictionary<Vector3Int, List<Vector3Int>>();
            foreach (var (key, nodes) in myHashGrid)
            {
                var nodesToCheck = new List<VerletNode>();
                int centerNodeCount = nodes.Count;
                foreach (var offset in SpatialHashGrid.offsets3D)
                {
                    var target = key + offset;
                    if (alreadyCheckedCells.ContainsKey(key))
                    {
                        if (checkDouble)
                        {
                            if (alreadyCheckedCells[key].Contains(target)) continue;
                        }
                        
                    }
                    if (myHashGrid.ContainsKey(target))
                    {
                        nodesToCheck.AddRange(myHashGrid[target]);
                        if (!alreadyCheckedCells.ContainsKey(target))
                        {
                            alreadyCheckedCells.Add(target, new List<Vector3Int>());
                        }
                        alreadyCheckedCells[target].Add(key);
                    }
                }
                SolveSelfCollision(nodesToCheck,centerNodeCount);
            }
        }

        private static void SolveSelfCollision(IList<VerletNode> myNodesToCheck, int myCenterNodeCount)
        {
            for (int i = 0; i < myCenterNodeCount; i++)
            {
                var nodeA = myNodesToCheck[i];
                for(var j = i+1; j<myNodesToCheck.Count; j++)
                {
                    var nodeB = myNodesToCheck[j];
                    
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
                        Debug.Log(nodeA.id + ", " + nodeB.id );
                    }
                    
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Verlet
{
    public class SelfCollision
    {
        public static bool checkDouble = true;

        public static void Solve(IList<VerletNode> nodes, Dictionary<Vector3Int, List<int>> hashGrid, float cellSize)
        {
            Vector3[] resultOffsets = new Vector3[nodes.Count];

            Parallel.For(0, nodes.Count, i =>
            {
                VerletNode currentNode = nodes[i];
                
                Vector3Int cellKey = SpatialHashGrid.GetCellKey(currentNode.Position, cellSize);
                foreach (var offset in SpatialHashGrid.offsets3D)
                {
                    if (!hashGrid.ContainsKey(cellKey + offset)) continue;
                    foreach (var otherIndex in hashGrid[cellKey + offset])
                    {
                        VerletNode otherNode = nodes[otherIndex];
                        Vector3 positionOffset = SolveNodes(currentNode, otherNode, out bool success);
                        if (success)
                        {
                            resultOffsets[i] += positionOffset;
                            resultOffsets[otherIndex] -= positionOffset;
                        }
                    }
                }
            });

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Position += resultOffsets[i];
            }
        }

        private static Vector3 SolveNodes(VerletNode active, VerletNode other, out bool success)
        {
            success = false;
            if (active == other) return Vector3.zero;
            if (active.Connection.Count != 12) return Vector3.zero;
            
            // TODO: figure out the neighborhood
            if(other.In(
                   active.DirectNeighbors
                   ))
            {
                return Vector3.zero;
            }

            
            Vector3 delta = active.Position - other.Position;
            float distanceSqr = delta.sqrMagnitude;
            float minDistanceSqr = Mathf.Pow(active.CollisionRadius + other.CollisionRadius, 2);

            if (distanceSqr > minDistanceSqr)
            {
                return Vector3.zero;
            }

            float distance = Mathf.Sqrt(distanceSqr);
            float minDistance = active.CollisionRadius + other.CollisionRadius;
            float offsetMagnitude = minDistance - distance;
            Vector3 direction = delta.normalized;
            
            success = true;
            return 0.5f * offsetMagnitude * direction;
        }

        private static VerletNode[] DirectConnections(VerletNode center)
        {
            return new[]
            {
                center.Neighbors[0],
                center.Neighbors[1],
                center.Neighbors[2],
                center.Neighbors[3],
                center.ParentStitch?.GetCorner(Stitch.NodeCorner.TopRight),
                center.Traverse(VerletNode.Neighbor.right)?.Traverse(VerletNode.Neighbor.down),
                center.Traverse(VerletNode.Neighbor.left)?.Traverse(VerletNode.Neighbor.down),
                center.Traverse(VerletNode.Neighbor.left)?.Traverse(VerletNode.Neighbor.up)
            };
        }
        
        /*public static void Collide(Dictionary<Vector3Int,List<VerletNode>> myHashGrid)
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
                    }
                    
                }
            }
        }*/
    }
}
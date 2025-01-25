using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

namespace Verlet
{
    public class SelfCollision
    {
        public static bool checkDouble = true;

        public static void Solve(IList<VerletNode> nodes, Dictionary<Vector3Int, List<int>> hashGrid, float cellSize)
        {

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
                        
                        SolveNodes(currentNode,otherNode);
                    }
                }
            });
        }

        private static void SolveNodes(VerletNode active, VerletNode other)
        {
            if (active == other) return;
            if (active.Connection.Count != 12) return;
            
            // TODO: figure out the neighborhood
            if(other.In(
                   active.DirectNeighbors
                   ))
            {
                return;
            }
            
            Vector3 delta = active.Position - other.Position;
            float distanceSqr = delta.sqrMagnitude;
            float minDistanceSqr = Mathf.Pow(active.CollisionRadius + other.CollisionRadius, 2);

            if (distanceSqr > minDistanceSqr)
            {
                return;
            }

            float distance = Mathf.Sqrt(distanceSqr);
            float minDistance = active.CollisionRadius + other.CollisionRadius;
            float offsetMagnitude = minDistance - distance;
            Vector3 direction = delta.normalized;
            active.Position+= 0.2f * offsetMagnitude * direction;
            other.Position -= 0.2f * offsetMagnitude * direction;
        }
    }
}
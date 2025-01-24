using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Verlet;

namespace DefaultNamespace
{
    
    public class SpatialHashGrid
    {
        public static Dictionary<Vector3Int,List<T>> Partition<T>(IEnumerable<T> items, Func<T, Vector3> positionGetter, float cellSize = 5f)
        {
            Dictionary<Vector3Int, List<T>> result = new Dictionary<Vector3Int, List<T>>();
            foreach (var item in items)
            {
                Vector3 pos = positionGetter(item);
                Vector3Int cellKey = new Vector3Int(GetCellKey(pos.x, cellSize), GetCellKey(pos.y,cellSize), GetCellKey(pos.z,cellSize));
                if (!result.ContainsKey(cellKey))
                {
                    result.Add(cellKey,new List<T>());
                }
                result[cellKey].Add(item);
            }
            Debug.Log(result.Values.Sum(item => item.Count));
            return result;
        }

        private static int GetCellKey(float value, float cellSize)
        {
            return Mathf.FloorToInt(value / cellSize);
        }

        public static Vector3Int[] offsets3D = new[]
        {//THIS DOES INCLUDE THE CENTER CELL
            new Vector3Int(-1, -1, -1),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, -1, 1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(0, -1, -1),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, -1, 1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, -1, -1),
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, -1, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 1, -1),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 1, 1),
            new Vector3Int(0,0,0)
        };
    }
}
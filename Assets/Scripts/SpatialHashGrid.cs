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
                Vector3Int cellKey = GetCellKey(pos, cellSize);
                if (!result.ContainsKey(cellKey))
                {
                    result.Add(cellKey,new List<T>());
                }
                result[cellKey].Add(item);
            }
            return result;
        }
        
        public static Dictionary<Vector2Int,List<T>> PartitionScreen<T>(IEnumerable<T> items, Func<T, Vector3> positionGetter, float cellSize = 5f)
        {
            Dictionary<Vector2Int, List<T>> result = new Dictionary<Vector2Int, List<T>>();
            foreach (var item in items)
            {
                Vector2 pos = positionGetter(item);
                Vector2Int cellKey = GetCellKey2D(pos,cellSize);
                if (!result.ContainsKey(cellKey))
                {
                    result.Add(cellKey,new List<T>());
                }
                result[cellKey].Add(item);
            }
            return result;
        }

        public static Dictionary<Vector3Int, List<int>> PartitionIndex<T>(IList<T> items, Func<T, Vector3> positionGetter, float cellSize = 5f)
        {
            Dictionary<Vector3Int, List<int>> result = new();
            for(int i = 0; i < items.Count; i++)
            {
                Vector3 pos = positionGetter(items[i]);
                Vector3Int cellKey = GetCellKey(pos, cellSize);
                if (!result.ContainsKey(cellKey))
                {
                    result[cellKey] = new();
                }
                result[cellKey].Add(i);
            }
            return result;
        }

        private static int GetCellKey(float value, float cellSize)
        {
            return Mathf.FloorToInt(value / cellSize);
        }

        public static Vector3Int GetCellKey(Vector3 position, float cellSize)
        {
            return new Vector3Int(GetCellKey(position.x, cellSize), GetCellKey(position.y, cellSize), GetCellKey(position.z, cellSize));
        }
        public static Vector2Int GetCellKey2D(Vector3 position, float cellSize)
        {
            return new Vector2Int(GetCellKey(position.x, cellSize), GetCellKey(position.y, cellSize));
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
        public static Vector2Int[] offsets2D = new[]
        {//THIS DOES INCLUDE THE CENTER CELL
            new Vector2Int(-1,-1),
            new Vector2Int(-1,0),
            new Vector2Int(-1,1),
            new Vector2Int(0,-1),
            new Vector2Int(0,0),
            new Vector2Int(0,1),
            new Vector2Int(1,-1),
            new Vector2Int(1,0),
            new Vector2Int(1,1),
        };
    }
}
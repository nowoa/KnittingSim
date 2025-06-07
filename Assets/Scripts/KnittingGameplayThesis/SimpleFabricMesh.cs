using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFabricMesh : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _meshFilter;

    void Start()
    {
        _meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    public void RegenerateMesh(IList<KnittingGameManager.SimpleStitch> myStitches)
    {
        //runs whenever there is a change to the mesh (remove/add stitch, change texture etc)
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
        _mesh = new Mesh();
        var structure = MeshStructure(myStitches);
        var vertexCount = structure.uvs.Count;
        _mesh.SetVertices(new Vector3[vertexCount]);
        _mesh.SetNormals(new Vector3[vertexCount]);
        _mesh.SetTriangles(structure.triangles,0);
        _mesh.SetUVs(0,structure.uvs);
        _mesh.name = "fabric";
        _meshFilter.sharedMesh = _mesh;
    }

    private static (List<int> triangles, List<Vector2> uvs) MeshStructure(IList<KnittingGameManager.SimpleStitch> myStitches)
    {
        //reset values
        var vertexIndex = 0;
        var triangleList = new List<int>();
        var uvList = new List<Vector2>();
        foreach (var s in myStitches)
        {
            var triangles = TrianglesFromStitch(s, ref vertexIndex);
            triangleList.AddRange(triangles);
            var UVs = UVsFromStitch(s);
            uvList.AddRange(UVs);
        }
        return (triangleList, uvList);
    }

    private static int[] TrianglesFromStitch(KnittingGameManager.SimpleStitch s, ref int myVertexIndex)
    {
        var result = new[]
        {
            myVertexIndex, myVertexIndex + 1, myVertexIndex + 2, myVertexIndex, myVertexIndex + 2,
            myVertexIndex + 3
        };
        myVertexIndex += 4;
        return result;
    }

    private static Vector2[] UVsFromStitch(KnittingGameManager.SimpleStitch s)
    {
          return new []{new Vector2(1,0), new Vector2(1,1), new Vector2(0,1), new Vector2(0,0)};
    }

    public void UpdatePositions(Vector3[] myVertices)
    { //runs every frame
        
        if (_mesh is null)
        {
            return;
        }
        _mesh.vertices = myVertices;
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();
    }

    public void DestroyMesh()
    {
        Destroy(_mesh);
    }
}

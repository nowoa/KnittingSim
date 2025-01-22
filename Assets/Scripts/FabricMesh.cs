using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using Verlet;

public class FabricMesh : MonoBehaviour
{
    private Mesh _mesh;
    private MeshFilter _meshFilter;

    void Start()
    {
        _meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    public void RegenerateMesh(IList<Stitch> myStitches)
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

    private static (List<int> triangles, List<Vector2> uvs) MeshStructure(IList<Stitch> myStitches)
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

    private static int[] TrianglesFromStitch(Stitch s, ref int myVertexIndex)
    {
        switch (s.stitchType)
        {
            case Stitch.StitchType.NORMAL:
                var result = new[]
                {
                    myVertexIndex, myVertexIndex + 1, myVertexIndex + 2, myVertexIndex, myVertexIndex + 2,
                    myVertexIndex + 3
                };
                myVertexIndex += 4;
                 return result;
        }

        return Array.Empty<int>();
    }

    private static Vector2[] UVsFromStitch(Stitch s)
    {
        if (s.Knit)
        {
          return new []{new Vector2(0.49f,0), new Vector2(0.49f,1), new Vector2(0,1), new Vector2(0,0)};
        }
        else
        {
          return new []{new Vector2(1f,0), new Vector2(1f,1), new Vector2(0.51f,1), new Vector2(0.51f,0)};
        }
    }

    public void UpdatePositions(Vector3[] myVertices, Vector3[] myNormals)
    { //runs every frame
        
        if (_mesh == null)
        {
            return;
        }

        _mesh.vertices = myVertices;
        _mesh.normals = myNormals;
    }

    
}

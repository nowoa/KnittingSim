using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    private Mesh _mesh;
    private MeshManager _meshManager;
    private MeshFilter _meshFilter;
    void Start()
    {
        _meshManager = gameObject.GetComponent<MeshManager>();
        _meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMesh()
    {
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
        Debug.Log("generate mesh");
        _mesh = new Mesh();
        _mesh.SetVertices(GetVerticesAndTriangles().vertices);
        _mesh.SetTriangles(GetVerticesAndTriangles().triangles,0);
        _mesh.RecalculateNormals();

        _meshFilter.sharedMesh = _mesh;
    }

    private (List<Vector3> vertices, List<int> triangles) GetVerticesAndTriangles()
    {
        var vertexIndex = 0;
        var vertexList = new List<Vector3>();
        var triangleList = new List<int>();
        foreach (var s in FabricManager.AllStitches)
        {
            vertexList.AddRange(new[] { s.corners[0].Position, s.corners[1].Position, s.corners[2].Position, s.corners[3].Position });
            triangleList.AddRange( new[] {vertexIndex, vertexIndex+1, vertexIndex +2, vertexIndex, vertexIndex+2, vertexIndex+3});
            vertexIndex +=4;
        }

        return (vertexList,triangleList);
    }
}

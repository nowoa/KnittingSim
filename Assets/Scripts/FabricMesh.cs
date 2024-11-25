using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verlet;

public class FabricMesh : MonoBehaviour
{
    private Mesh _mesh;
    private FabricMesh _fabricMesh;
    private MeshFilter _meshFilter;
    void Start()
    {
        _fabricMesh = gameObject.GetComponent<FabricMesh>();
        _meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update() 
    {
        
    }

    public void UpdateMesh()
    {
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
        _mesh = new Mesh();
        _mesh.SetVertices(GetVerticesAndTriangles().vertices);
        _mesh.SetTriangles(GetVerticesAndTriangles().triangles,0);
        _mesh.SetNormals(GetVerticesAndTriangles().normals);

        _meshFilter.sharedMesh = _mesh;
    }

    private (List<Vector3> vertices, List<int> triangles, List<Vector3> normals) GetVerticesAndTriangles()
    {
        Color color;
        var vertexIndex = 0;
        var vertexList = new List<Vector3>();
        var triangleList = new List<int>();
        var normalList = new List<Vector3>();
        foreach (var n in FabricManager.AllNodes)
        {
            n.CalculateNormal();
        }
        foreach (var s in FabricManager.AllStitches)
        {
            s.SetParentMesh(this);
            if (s.isInactive)
            {
                continue;
            }
            vertexList.AddRange(new[] { s.corners[0].Position, s.corners[1].Position, s.corners[2].Position, s.corners[3].Position });
            triangleList.AddRange( new[] {vertexIndex, vertexIndex+1, vertexIndex +2, vertexIndex, vertexIndex+2, vertexIndex+3});
            normalList.AddRange(new []{s.corners[0].normal,s.corners[1].normal,s.corners[2].normal,s.corners[3].normal});
            vertexIndex +=4;
        }

        return (vertexList,triangleList, normalList);
    }
    
    public void RenderNodes(Material material, Mesh mesh)
    {
        var rparams = new RenderParams(material);
        List<Matrix4x4> renderMatrices = new();
        foreach (var node in FabricManager.AllNodes)
        {
            Matrix4x4 mat = Matrix4x4.TRS(node.Position, Quaternion.identity, Vector3.one * 0.1f);
            renderMatrices.Add(mat);
        }
        Graphics.RenderMeshInstanced(rparams, mesh, 0, renderMatrices);
    }

    public void UpdatePositions()
    {
        
        if (_mesh == null)
        {
            return;
        }
        var vertexList = new List<Vector3>();
        var normalList = new List<Vector3>();
        foreach (var n in FabricManager.AllNodes)
        {
            n.CalculateNormal();
        }
        foreach (var s in FabricManager.AllStitches)
        {
            if (s.isInactive)
            {
                continue;
            }
            vertexList.AddRange(new []{s.corners[0].Position,s.corners[1].Position,s.corners[2].Position,s.corners[3].Position});
            normalList.AddRange(new []{s.corners[0].normal,s.corners[1].normal,s.corners[2].normal,s.corners[3].normal});
        }

        _mesh.SetVertices(vertexList);
        _mesh.SetNormals(normalList);
    }
}

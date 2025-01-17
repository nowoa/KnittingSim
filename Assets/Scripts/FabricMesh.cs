using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verlet;

public class FabricMesh : MonoBehaviour
{
    private Mesh _mesh;
    private FabricMesh _fabricMesh;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private List<Vector3> _vertexList;
    private List<int> _triangleList;
    private List<Vector3> _normalList;
    private List<Vector2> _uvList;
    private int _vertexIndex;
    void Start()
    {
        _fabricMesh = gameObject.GetComponent<FabricMesh>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    public void UpdateMesh()
    {
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
        _mesh = new Mesh();
        
        _mesh.SetVertices(GetMeshInfo().vertices);
        _mesh.SetTriangles(GetMeshInfo().triangles,0);
        _mesh.SetNormals(GetMeshInfo().normals);
        _mesh.SetUVs(0,GetMeshInfo().uvs);
        _mesh.name = "fabric";
        
        _meshFilter.sharedMesh = _mesh;
    }

    private (List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs) GetMeshInfo()
    {
        //reset values
        _vertexIndex = 0;
        _vertexList = new List<Vector3>();
        _triangleList = new List<int>();
        _normalList = new List<Vector3>();
        _uvList = new List<Vector2>();
        
        var nodes = GameManager.Instance.Project.Nodes;
        Stitch[] stitches = GameManager.Instance.Project.GetPanels().SelectMany(item => item.Stitches).ToArray();
        
        foreach (var s in stitches)
        {
            s.CalculateNormal();
        }
        foreach (var n in nodes)
        {
            n.CalculateNormal();
        }
        foreach (var s in stitches)
        {
            GetMeshParams(s);
            GetUVs(s);
        }

        return (_vertexList,_triangleList, _normalList, _uvList);
    }

    private void GetMeshParams(Stitch s)
    {
        switch (s.stitchType)
        {
            case Stitch.StitchType.NORMAL:
                _vertexList.AddRange(MeshInfo(s,_vertexIndex, Stitch.StitchType.NORMAL).vertices);
                _triangleList.AddRange(MeshInfo(s,_vertexIndex, Stitch.StitchType.NORMAL).triangles);
                _normalList.AddRange(MeshInfo(s,_vertexIndex, Stitch.StitchType.NORMAL).normals);
                _vertexIndex += 4;
                break;
        }
    }

    private void GetUVs(Stitch s)
    {
        if (s.isKnit)
        {
            _uvList.AddRange(new []{new Vector2(0.49f,0), new Vector2(0.49f,1), new Vector2(0,1), new Vector2(0,0)});
        }
        else
        {
            _uvList.AddRange(new []{new Vector2(1f,0), new Vector2(1f,1), new Vector2(0.51f,1), new Vector2(0.51f,0)});
        }
    }

    private (Vector3[] vertices, int[] triangles, Vector3[] normals) MeshInfo(Stitch s, int vertexIndex, Stitch.StitchType myStitchType)
    {
        switch (myStitchType)
        {
            default: //currently only has one case, but should handle different stitch types differently (e.g. decreases)
            var vertices = new[] {s.Corners[0].Position, s.Corners[1].Position, s.Corners[2].Position, s.Corners[3].Position};
            var triangles = new[]
                { vertexIndex, vertexIndex + 1, vertexIndex + 2, vertexIndex, vertexIndex + 2, vertexIndex + 3 };
            var normals = new[] { s.Corners[0].Normal, s.Corners[1].Normal, s.Corners[2].Normal, s.Corners[3].Normal };
            return (vertices, triangles, normals);
        }
    }

    public void UpdatePositions()
    {
        //runs every frame
        
        if (_mesh == null)
        {
            return;
        }
        _vertexList = new List<Vector3>();
        _normalList = new List<Vector3>();

        var nodes = GameManager.Instance.Project.Nodes;
        Stitch[] stitches = GameManager.Instance.Project.GetPanels().SelectMany(item => item.Stitches).ToArray();
        
        foreach (var s in stitches)
        {
            s.CalculateNormal();
        }
        foreach (var n in nodes)
        {
            n.CalculateNormal();
        }
        foreach (var s in stitches)
        {
            //TODO: update the vertex list taking into account decreased stitches having only 3 verts
            var corners = GetCorners(s);
            _vertexList.AddRange(corners.Select(item => item.Position));
            _normalList.AddRange(corners.Select(item => item.Normal));
        }

        _mesh.SetVertices(_vertexList);
        _mesh.SetNormals(_normalList);
    }

    private VerletNode[] GetCorners(Stitch s)
    {
        var type = s.stitchType;
        switch (type)
        {
            case Stitch.StitchType.NORMAL:
                return new[]
                    { s.Corners[0], s.Corners[1], s.Corners[2], s.Corners[3]};
            default:
                return new[] { s.Corners[0], s.Corners[1], s.Corners[2], s.Corners[3] };
        }
    }
}

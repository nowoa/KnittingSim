using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.VisualScripting;
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
        _mesh.SetVertices(GetMeshInfo().vertices);
        _mesh.SetTriangles(GetMeshInfo().triangles,0);
        _mesh.SetNormals(GetMeshInfo().normals);
        _mesh.SetUVs(0,GetMeshInfo().uvs);

        _meshFilter.sharedMesh = _mesh;
    }

    private (List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs) GetMeshInfo()
    {
        var vertexIndex = 0;
        var vertexList = new List<Vector3>();
        var triangleList = new List<int>();
        var normalList = new List<Vector3>();
        var uvList = new List<Vector2>();
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

            switch (s.stitchType)
            {
                case StitchInfo.StitchType.normal:
                    vertexList.AddRange(MeshInfoNormal(s,vertexIndex).vertices);
                    triangleList.AddRange(MeshInfoNormal(s,vertexIndex).triangles);
                    normalList.AddRange(MeshInfoNormal(s,vertexIndex).normals);
                    vertexIndex += 4;
                    break;
                case StitchInfo.StitchType.DecreaseFirst:
                    vertexList.AddRange(MeshInfoDecrease(s,vertexIndex).vertices);
                    triangleList.AddRange(MeshInfoDecrease(s,vertexIndex).triangles);
                    normalList.AddRange(MeshInfoDecrease(s,vertexIndex).normals);
                    vertexIndex += 3;
                    break;
                case StitchInfo.StitchType.DecreaseMiddle:
                    vertexList.AddRange(MeshInfoDecrease(s,vertexIndex).vertices);
                    triangleList.AddRange(MeshInfoDecrease(s,vertexIndex).triangles);
                    normalList.AddRange(MeshInfoDecrease(s,vertexIndex).normals);
                    vertexIndex += 3;
                    break;
                default:
                    vertexList.AddRange(MeshInfoNormal(s,vertexIndex).vertices);
                    triangleList.AddRange(MeshInfoNormal(s,vertexIndex).triangles);
                    normalList.AddRange(MeshInfoNormal(s,vertexIndex).normals);
                    vertexIndex += 4;
                    break;
            }
            /*if (s.Knit)
            {
                uvList.AddRange(new []{new Vector2(0.49f,0), new Vector2(0.49f,1), new Vector2(0,1), new Vector2(0,0)});
            }
            else
            {
                uvList.AddRange(new []{new Vector2(1f,0), new Vector2(1f,1), new Vector2(0.51f,1), new Vector2(0.51f,0)});
            }*/
        }

        return (vertexList,triangleList, normalList, uvList);
    }

    private (Vector3[] vertices, int[] triangles, Vector3[] normals) MeshInfoNormal(StitchInfo s, int vertexIndex)
    {
        var vertices = new[] {s.corners[0].Position, s.corners[1].Position, s.corners[2].Position, s.corners[3].Position};
        var triangles = new[]
            { vertexIndex, vertexIndex + 1, vertexIndex + 2, vertexIndex, vertexIndex + 2, vertexIndex + 3 };
        var normals = new[] { s.corners[0].normal, s.corners[1].normal, s.corners[2].normal, s.corners[3].normal };
        return (vertices, triangles, normals);
    }

    private (Vector3[] vertices, int[] triangles, Vector3[] normals) MeshInfoDecrease(StitchInfo s, int vertexIndex)
    {
        var vertices = new[] { s.corners[0].Position, s.corners[1].Position, s.corners[2].Position };
        var triangles = new[] { vertexIndex, vertexIndex + 1, vertexIndex + 2 };
        var normals = new[] { s.corners[0].normal, s.corners[1].normal, s.corners[2].normal };
        return (vertices, triangles, normals);
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
            //TODO: update the vertex list taking into account decreased stitches having only 3 verts
            vertexList.AddRange(new []{s.corners[0].Position,s.corners[1].Position,s.corners[2].Position,s.corners[3].Position});
            normalList.AddRange(new []{s.corners[0].normal,s.corners[1].normal,s.corners[2].normal,s.corners[3].normal});
        }

        _mesh.SetVertices(vertexList);
        _mesh.SetNormals(normalList);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public Vector3 x = new Vector3(99999.99999f,0);
    public Vector3 y = new Vector3(0, 99999.99999f);

    public Material defaultMaterial;
    
    private IEnumerator Start()
    {
        yield return null;
        StartGeneration();
    }

    private void StartGeneration()
    {
        var areas = SubdivideIntoZones();
        
        foreach (var area in areas)
        {
            area.Material = defaultMaterial;
            area.ScaleFactor = new Vector2(0.5f,0.5f);
            area.Generate();
        }
    }

    Zone[] SubdivideIntoZones()
    {
        return new[]
        {
            new Zone(
                10,10
            ),
            new Zone(
            10,10,
            10,10
            ),
            new Zone(
                10,10,
                0,10
            ),
            new Zone(
                10,10,
                10,0
            )
        };
    }
}

public class Zone
{
    public Material Material;
    public Vector2 ScaleFactor = new Vector2(1,1);
    
    private readonly int _lengthX;
    private readonly int _widthZ;

    private readonly int _offsetX;
    private readonly int _offsetZ;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Vector2[] _uvs;
    private int[] _triangles;

    public Zone(int lengthX, int widthZ)
    {
        _lengthX = lengthX;
        _widthZ = widthZ;
    }

    public Zone(int lengthX, int widthZ, int offsetX, int offsetZ)
    {
        _lengthX = lengthX;
        _widthZ = widthZ;
        _offsetX = offsetX;
        _offsetZ = offsetZ;
    }

    public void Generate()
    {
        GenerateMesh();
        CreateObject();
    }

    private void CreateObject()
    {
        var zoneObject = new GameObject();
        var position = new Vector3(_offsetX + _lengthX, 0,_offsetZ + _widthZ);
        
        zoneObject.transform.position = position;
        zoneObject.AddComponent<MeshFilter>().mesh = _mesh;
        zoneObject.AddComponent<MeshRenderer>().material = Material;
    }

    void GenerateMesh()
    {
        _vertices = GenerateVertices();
        _triangles = GenerateTriangles();
        _normals = SetUpNormals();
        
        _uvs = GenerateUV();
        _uvs = ScaleUV(ScaleFactor);
        
        _mesh = new Mesh();
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.uv = _uvs;
        _mesh.normals = _normals;
    }
    
    protected virtual Vector3[] GenerateVertices()
    {
        var vertices = new List<Vector3>();
        
        for (int x = 0; x <= _lengthX; x++)
        {
            for (int z = 0; z <= _widthZ; z++)
            {
                var triangle = new Vector3(x, 0, z);
                vertices.Add(triangle);
            } 
        }
        
        return vertices.ToArray();
    }
    protected virtual int[] GenerateTriangles()
    {
        var triangles = new List<int>();

        for (int i = 0; i <= _vertices.Length; i++)
        {
            int offset = i / _widthZ;
            var index = i - offset;
            
            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + _widthZ + 1);

            triangles.Add(index + _widthZ);
            triangles.Add(index);
            triangles.Add(index + _widthZ + 1);
        }

        return triangles.ToArray();
    }
    protected virtual Vector2[] GenerateUV()
    {
        Vector2[] uvs = new Vector2[_vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(_vertices[i].x, _vertices[i].z);
        }
        
        return uvs;
    }
    protected virtual Vector2[] ScaleUV(Vector2 scaleFactor)
    {
        for (int i = 0; i < _uvs.Length; i++)
        {
            _uvs[i] = Vector2.Scale(_uvs[i], scaleFactor);
        }
        
        return _uvs;
    }
    protected virtual Vector3[] SetUpNormals()
    {
        var normals = new Vector3[_vertices.Length];
        
        for (var index = 0; index < normals.Length; index++)
        {
            normals[index] = Vector3.up;
        }
        
        return normals;
    }
}

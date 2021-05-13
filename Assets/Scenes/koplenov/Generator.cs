using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    public Vector3 x = new Vector3(99999.99999f,0);
    public Vector3 y = new Vector3(0, 99999.99999f);

    public Material defaultMaterial;
    public GameObject[] decorations;

    public GameObject Player;
    
    private IEnumerator Start()
    {
        yield return null;
        StartGeneration();
        
        // for AI
        gameObject.AddComponent<NavMeshSurface>().BuildNavMesh();

        Player.SetActive(true);
    }

    private void StartGeneration()
    {
        var areas = SubdivideIntoZones();
        
        foreach (Zone area in areas)
        {
            area.Material = defaultMaterial;
            area.Decorations = decorations;
            
            area.ScaleFactor = new Vector2(0.5f,0.5f);
            area.Generate();
        }
    }

    Zone[] SubdivideIntoZones()
    {
        return new[]
        {
            new Zone(transform,
                16,10
            ),
            new Zone(transform,
            40,40,
            -20,-20
            )/*,
            new Zone(transform,
                10,10,
                0,10
            ),
            new Zone(transform,
                10,10,
                10,0
            )*/
        };
    }
}

public class Zone
{
    #region Random

    private static System.Random _generator = null; 
    public void SplitAtRandom(int chanceOfSuccess, Action onSuccess, Action onFailure)
    {
        // Seed
        if (_generator == null)
            _generator = new System.Random(DateTime.Now.Millisecond);

        // By chance
        if (_generator.Next(100) < chanceOfSuccess)
        {
            if (onSuccess != null)
                onSuccess();
        }
        else
        {
            if (onFailure != null)
                onFailure();
        }
    }

    #endregion

    public GameObject ZoneObject;
    public GameObject[] Decorations;
    private Transform _root;
    
    
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

    public Zone(Transform root, int lengthX, int widthZ)
    {
        _root = root; 
        _lengthX = lengthX;
        _widthZ = widthZ;
    }

    public Zone(Transform root, int lengthX, int widthZ, int offsetX, int offsetZ)
    {
        _root = root; 
        
        _lengthX = lengthX;
        _widthZ = widthZ;
        _offsetX = offsetX;
        _offsetZ = offsetZ;
    }

    public void Generate()
    {
        GenerateMesh();
        CreateObject();
        AddDecorations();
    }

    private void AddDecorations()
    {
        var decorationsGameObject = new GameObject("Decorations");
        decorationsGameObject.transform.parent = ZoneObject.transform;
        
        for (var index = 0; index < _vertices.Length; index+=4)
        {
            SplitAtRandom(15,//%
                () =>
                {
                    Vector3 point = ZoneObject.transform.TransformPoint(_vertices[index]);
                    var instance = Object.Instantiate(Decorations[Random.Range(0, Decorations.Length)], point, Quaternion.identity, decorationsGameObject.transform);
                    instance.AddComponent<NavMeshModifier>();
                    instance.isStatic = true;
                }, 
                () => { /*  ¯\_(ツ)_/¯ */ });
        }
        
        
        MeshCombiner meshCombiner = decorationsGameObject.AddComponent<MeshCombiner>();
        meshCombiner.DestroyCombinedChildren = true;
        meshCombiner.CreateMultiMaterialMesh = true;
        meshCombiner.CombineMeshes(false);
    }

    private void CreateObject()
    {
        var zoneObject = new GameObject("Zone");
        // daddy....
        zoneObject.transform.parent = _root;

        var position = new Vector3(_offsetX + _lengthX, 0,_offsetZ + _widthZ);
        
        zoneObject.transform.position = position;
        zoneObject.AddComponent<MeshFilter>().mesh = _mesh;
        zoneObject.AddComponent<MeshRenderer>().sharedMaterial = Material;

        // For Ai
        zoneObject.AddComponent<MeshCollider>();

        // link
        ZoneObject = zoneObject;
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

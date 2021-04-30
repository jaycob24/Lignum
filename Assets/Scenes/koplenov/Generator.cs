using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public Vector3 x = new Vector3(99999.99999f,0);
    public Vector3 y = new Vector3(0, 99999.99999f);

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
            area.Generate();
        }
        
        // single test
        //areas[0].Generate();
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
    private readonly int _lengthX;
    private readonly int _widthZ;

    private readonly int _offsetX;
    private readonly int _offsetZ;

    private Mesh _mesh;
    private Vector3[] _vertices;
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
        zoneObject.AddComponent<MeshRenderer>();
    }

    void GenerateMesh()
    {
        _mesh = new Mesh();

        /*
        _vertices = new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0)
        };

        _triangles = new[]
        {
            0, 1, 2
        };
        */
        _vertices = GenerateVertices();
        _triangles = GenerateTriangles();
        
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        
        _mesh.RecalculateNormals();
    }
    
    public virtual Vector3[] GenerateVertices()
    {
        var vertices = new List<Vector3>();
        
        //vertices.Add(new Vector3(0, 0, 0));
        
        for (int x = 0; x <= _lengthX; x++)
        {
            for (int z = 0; z <= _widthZ; z++)
            {
                var triangle = new Vector3(x, 0, z);
                vertices.Add(triangle);
                
                //Debug.Log(triangle);
                //var debugObject = new GameObject();
                //debugObject.transform.position = triangle;
            } 
        }
        return vertices.ToArray();
    }

    public virtual int[] TempGenerateTriangles()
    {
        var triangles = new List<int>();
        /*
        for (int z = 0; z <= _lengthX/2; z++)
        {
            for (int index = z +_widthZ; index < _vertices.Length/_lengthX * (z + 1); index++)
            {
                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + _widthZ);

                //triangles.Add(index + _widthZ);
                //triangles.Add(index + 1);
                //triangles.Add(index + _widthZ + 1);
            }
        }
        */
// 
        triangles.Add(0);
        triangles.Add(0 + 1);
        triangles.Add(0 + _widthZ);
//      
        triangles.Add(1);
        triangles.Add(1 + 1);
        triangles.Add(1 + _widthZ); 
        
        triangles.Add(2);
        triangles.Add(2 + 1);
        triangles.Add(2 + _widthZ);
        
        
        
        
        for (int index = 0; index < _widthZ; index += _widthZ)
        {
            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + _widthZ);

            //triangles.Add(index + _widthZ);
            //triangles.Add(index + 1);
            //triangles.Add(index + _widthZ + 1);
        }
        
        
        
        return triangles.ToArray();
    } 
    public virtual int[] GenerateTriangles()
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
}

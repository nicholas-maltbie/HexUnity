using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class IcosphereMesh : MonoBehaviour
{
    public float edgeLength = 1;
    public int subdivsions = 1;
    private MeshFilter meshFilter;
    private Mesh mesh;
    private Icosphere sphere;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshFilter>();
        gameObject.GetComponent<MeshRenderer>();

        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        sphere = new Icosphere(transform.position, 1);

        for (int sub = 0; sub < subdivsions; sub++)
            sphere = sphere.SubdivideSphere();

        List<SCoord> coordinates = new List<SCoord>(sphere.Coordinates);
        List<int> triangles = new List<int>();


        Vector3 v1 = sphere.GetPoint(coordinates[0]);
        IEnumerator<SCoord> scaleNeighbors = sphere.GetNeighbors(coordinates[0]).GetEnumerator();
        scaleNeighbors.MoveNext();
        Vector3 v2 = sphere.GetPoint(scaleNeighbors.Current);
        float dist = Vector3.Distance(v1, v2);

        float sf = edgeLength / dist;
        sphere.SetRadius(sf);


        Vector3[] vertices = new Vector3[coordinates.Count];
        Vector3[] normals = new Vector3[coordinates.Count];
        Dictionary<SCoord, int> keyLookup = new Dictionary<SCoord, int>();
        for (int i = 0; i < coordinates.Count; i++)
        {
            vertices[i] = sphere.GetPoint(coordinates[i]);
            keyLookup[coordinates[i]] = i;
            normals[i] = coordinates[i].ToEuclidian();
        }
        SCoordComparatorLon coordSort = new SCoordComparatorLon();

        for (int i = 0; i < coordinates.Count; i++)
        {
            SCoord coord = coordinates[i];
            LinkedList<SCoord> neighbors = new LinkedList<SCoord>(sphere.GetNeighbors(coord));

            SCoord startVertex = neighbors.First.Value;
            SCoord source = startVertex;
            neighbors.RemoveFirst();

            while (neighbors.Count > 1)
            {
                LinkedListNode<SCoord> nextVertex = neighbors.First;
                while (!new List<SCoord>(sphere.GetNeighbors(nextVertex.Value)).Contains(source))
                    nextVertex = nextVertex.Next;
                
                neighbors.Remove(nextVertex);

                if (keyLookup[nextVertex.Value] > i && keyLookup[source] > i)
                {
                    SCoord[] triangleCoords = SCoord.SortClockwiseOrder(coord, nextVertex.Value, source);

                    triangles.Add(keyLookup[triangleCoords[2]]);
                    triangles.Add(keyLookup[triangleCoords[1]]);
                    triangles.Add(keyLookup[triangleCoords[0]]);
                }

                source = nextVertex.Value;
            }

            if (keyLookup[neighbors.First.Value] > i && keyLookup[source] > i)
            {
                SCoord[] triangleCoords = SCoord.SortClockwiseOrder(coord, neighbors.First.Value, source);

                triangles.Add(keyLookup[triangleCoords[2]]);
                triangles.Add(keyLookup[triangleCoords[1]]);
                triangles.Add(keyLookup[triangleCoords[0]]);
            }

            if (keyLookup[neighbors.First.Value] > i && keyLookup[startVertex] > i)
            {
                SCoord[] triangleCoords = SCoord.SortClockwiseOrder(coord, neighbors.First.Value, startVertex);

                triangles.Add(keyLookup[triangleCoords[2]]);
                triangles.Add(keyLookup[triangleCoords[1]]);
                triangles.Add(keyLookup[triangleCoords[0]]);
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;

        Debug.Log(triangles.Count);

        //mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = triangles.ToArray();
    }


    // Update is called once per frame
    void Update()
    {
        /*foreach (SCoord coord in sphere.Coordinates)
        {
            foreach (SCoord other in sphere.GetNeighbors(coord))
            {
                Debug.DrawLine(sphere.GetPoint(coord), sphere.GetPoint(other), Color.blue);
            }
        }*/
    }
}

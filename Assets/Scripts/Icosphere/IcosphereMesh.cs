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
    private Vector3[] vertices;
    private List<SCoord> coordinates;
    private Dictionary<SCoord, int> keyLookup;
    private int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        // Get the mst filter component
        meshFilter = GetComponent<MeshFilter>();
        // Save the mesh
        mesh = meshFilter.mesh;

        // Make and subdivide the icosphere
        sphere = new Icosphere(transform.position, 1);
        for (int sub = 0; sub < subdivsions; sub++)
            sphere = sphere.SubdivideSphere();

        // Make a list of the coordinates
        coordinates = new List<SCoord>(sphere.Coordinates);
        // Make a list to put the tranle faces into.
        List<int> triangleList = new List<int>();

        // Compute the scaling factor to meet the target edge length
        // Get the distance from the pole to one of its neighbors
        Vector3 scaleVec1 = sphere.GetPoint(coordinates[0]);
        IEnumerator<SCoord> scaleNeighbors = sphere.GetNeighbors(coordinates[0]).GetEnumerator();
        scaleNeighbors.MoveNext();
        Vector3 scaleVec2 = sphere.GetPoint(scaleNeighbors.Current);
        float dist = Vector3.Distance(scaleVec1, scaleVec2);

        // Compute the new scale factor and set this for the sphere
        float sf = edgeLength / dist;
        sphere.SetRadius(sf);

        // Comptue the 3d space of the different vectors
        vertices = new Vector3[coordinates.Count];
        // Saved list of normals for each vertex
        Vector3[] normals = new Vector3[coordinates.Count];
        // Save reverselookup of SCoord to index in array of vertices
        keyLookup = new Dictionary<SCoord, int>();
        // For each vetex, compute the 3d position, normal and save in lookup table
        for (int i = 0; i < coordinates.Count; i++)
        {
            vertices[i] = sphere.GetPoint(coordinates[i]);
            keyLookup[coordinates[i]] = i;
            normals[i] = coordinates[i].ToEuclidian();
        }

        // For each coordinate, compute the associated faces
        for (int i = 0; i < coordinates.Count; i++)
        {
            // Get the coordinate
            SCoord coord = coordinates[i];

            // Make a linked list of the coordinate's neighbors
            LinkedList<SCoord> neighbors = new LinkedList<SCoord>(sphere.GetNeighbors(coord));

            // Start drawing triangles between the coordinate and the coordinate's neighbors
            SCoord startVertex = neighbors.First.Value;
            SCoord source = startVertex;
            neighbors.RemoveFirst();

            // For each neighbor after the first, join them in a triangle-fan like fashion
            while (neighbors.Count > 1)
            {
                // Find the next SCoord in the sequence of neighbors (neighbor to origin that is 
                // adjacent to the previous neighbor)
                LinkedListNode<SCoord> nextVertex = neighbors.First;
                while (!new List<SCoord>(sphere.GetNeighbors(nextVertex.Value)).Contains(source))
                    nextVertex = nextVertex.Next;

                // Check to make sure that this face has not been added already
                if (keyLookup[nextVertex.Value] > i && keyLookup[source] > i)
                {
                    // Sort vertices in clockwise order
                    SCoord[] triangleCoords = SCoord.SortClockwiseOrder(coord, nextVertex.Value, source);

                    triangleList.Add(keyLookup[triangleCoords[2]]);
                    triangleList.Add(keyLookup[triangleCoords[1]]);
                    triangleList.Add(keyLookup[triangleCoords[0]]);
                }

                // Save current vertex as previous vertex
                source = nextVertex.Value;
                // Remove current vertex so it is not checked again
                neighbors.Remove(nextVertex);
            }

            // Connext last vertex in sequence to previous vertex
            if (keyLookup[neighbors.First.Value] > i && keyLookup[source] > i)
            {
                // Sort vertices in clockwise order
                SCoord[] triangleCoords = SCoord.SortClockwiseOrder(coord, neighbors.First.Value, source);

                triangleList.Add(keyLookup[triangleCoords[2]]);
                triangleList.Add(keyLookup[triangleCoords[1]]);
                triangleList.Add(keyLookup[triangleCoords[0]]);
            }

            // Connext last vertex to first vertex
            if (keyLookup[neighbors.First.Value] > i && keyLookup[startVertex] > i)
            {
                // Sort vertices in clockwise order
                SCoord[] triangleCoords = SCoord.SortClockwiseOrder(coord, neighbors.First.Value, startVertex);

                triangleList.Add(keyLookup[triangleCoords[2]]);
                triangleList.Add(keyLookup[triangleCoords[1]]);
                triangleList.Add(keyLookup[triangleCoords[0]]);
            }
        }

        // Save list of verteices, normals and triangles in the mesh renderer. 
        mesh.vertices = vertices;
        mesh.normals = normals;
        triangles = triangleList.ToArray();

    }


    // Update is called once per frame
    void Update()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        /*foreach (SCoord coord in sphere.Coordinates)
        {
            foreach (SCoord other in sphere.GetNeighbors(coord))
            {
                Debug.DrawLine(sphere.GetPoint(coord), sphere.GetPoint(other), Color.blue);
            }
        }*/
    }
}

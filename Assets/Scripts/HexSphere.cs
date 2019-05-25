using System;
using System.Collections.Generic;
using UnityEngine;

public class HexSphere
{
    Icosphere hexSphere;
    private Dictionary<SCoord, GameObject> tileMap;

	public HexSphere(int subdivisions, Vector3 center, float edgeLength)
	{
        tileMap = new Dictionary<SCoord, GameObject>();

        // Make and subdivide the icosphere
        hexSphere = new Icosphere(center, 1);
        for (int sub = 0; sub < subdivisions; sub++)
            hexSphere = hexSphere.SubdivideSphere();

        // Scale the spehres
        float sf = HexSphere.GetScaleFactor(hexSphere, edgeLength);
        hexSphere.SetRadius(sf);
    }

    public void RenderSphere(Transform parentObject)
    {
        foreach(SCoord tile in hexSphere.Coordinates)
        {
            GameObject newTile = new GameObject();
            MeshFilter mf = newTile.AddComponent<MeshFilter>();
            MeshRenderer mr = newTile.AddComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Standard"));

            RenderTile(mf.mesh, tile);

            newTile.transform.position += parentObject.transform.position;
            newTile.transform.Rotate(parentObject.transform.eulerAngles);

            newTile.transform.SetParent(parentObject);

            tileMap[tile] = newTile;
        }
    }

    private static float GetScaleFactor(Icosphere sphere, float edgeLength)
    {
        List<SCoord> coordinates = new List<SCoord>(sphere.Coordinates);
        // Compute the scaling factor to meet the target edge length
        // Get the distance from the pole to one of its neighbors
        Vector3 scaleVec1 = sphere.GetPoint(coordinates[0]);
        IEnumerator<SCoord> scaleNeighbors = sphere.GetNeighbors(coordinates[0]).GetEnumerator();
        scaleNeighbors.MoveNext();
        Vector3 scaleVec2 = sphere.GetPoint(scaleNeighbors.Current);
        float dist = Vector3.Distance(scaleVec1, scaleVec2);

        // Compute the new scale factor and set this for the sphere
        float sf = edgeLength / dist;
        return sf;
    }

    private void RenderTile(Mesh mesh, SCoord tileCenter)
    {
        // Make a linked list of the coordinate's neighbors
        LinkedList<SCoord> neighbors;
        // hexes are not triangles
        neighbors = new LinkedList<SCoord>(hexSphere.GetNeighbors(tileCenter));

        // Make list of vertices
        List<Vector3> vertices = new List<Vector3>(neighbors.Count + 1);
        List<Vector3> normals = new List<Vector3>(neighbors.Count + 1);
        Dictionary<SCoord, int> keyLookup = new Dictionary<SCoord, int>();

        keyLookup.Add(tileCenter, keyLookup.Count);
        vertices.Add(hexSphere.GetPoint(tileCenter));
        normals.Add(tileCenter.ToEuclidian());

        SCoord startVertex = neighbors.First.Value;
        SCoord source = startVertex;
        neighbors.RemoveFirst();
        SCoord vert;

        SCoord[] triangleCoords;

        while (neighbors.Count > 1)
        {
            // Find the next SCoord in the sequence of neighbors (neighbor to origin that is 
            // adjacent to the previous neighbor)
            LinkedListNode<SCoord> nextVertex = neighbors.First;
            while (!new List<SCoord>(hexSphere.GetNeighbors(nextVertex.Value)).Contains(source))
                nextVertex = nextVertex.Next;

            // Sort vertices in clockwise order
            vert = SCoord.GetCentroid(tileCenter, nextVertex.Value, source);

            keyLookup.Add(source, keyLookup.Count);
            vertices.Add(hexSphere.GetPoint(vert));
            normals.Add(vert.ToEuclidian());

            // Save current vertex as previous vertex
            source = nextVertex.Value;
            // Remove current vertex so it is not checked again
            neighbors.Remove(nextVertex);
        }

        vert = SCoord.GetCentroid(tileCenter, neighbors.First.Value, source);
        keyLookup.Add(source, keyLookup.Count);
        vertices.Add(hexSphere.GetPoint(vert));
        normals.Add(vert.ToEuclidian());

        vert = SCoord.GetCentroid(tileCenter, neighbors.First.Value, startVertex);
        keyLookup.Add(neighbors.First.Value, keyLookup.Count);
        vertices.Add(hexSphere.GetPoint(vert));
        normals.Add(vert.ToEuclidian());


        // hexes are not triangles
        neighbors = new LinkedList<SCoord>(hexSphere.GetNeighbors(tileCenter));

        // Start drawing triangles between the coordinate and the coordinate's neighbors
        List<int> triangleList = new List<int>();

        startVertex = neighbors.First.Value;
        source = startVertex;
        neighbors.RemoveFirst();

        // For each neighbor after the first, join them in a triangle-fan like fashion
        while (neighbors.Count > 1)
        {
            // Find the next SCoord in the sequence of neighbors (neighbor to origin that is 
            // adjacent to the previous neighbor)
            LinkedListNode<SCoord> nextVertex = neighbors.First;
            while (!new List<SCoord>(hexSphere.GetNeighbors(nextVertex.Value)).Contains(source))
                nextVertex = nextVertex.Next;

            // Sort vertices in clockwise order
            triangleCoords = SCoord.SortClockwiseOrder(tileCenter, nextVertex.Value, source);

            triangleList.Add(keyLookup[triangleCoords[2]]);
            triangleList.Add(keyLookup[triangleCoords[1]]);
            triangleList.Add(keyLookup[triangleCoords[0]]);

            // Save current vertex as previous vertex
            source = nextVertex.Value;
            // Remove current vertex so it is not checked again
            neighbors.Remove(nextVertex);
        }

        // Connext last vertex in sequence to previous vertex

        // Sort vertices in clockwise order
        triangleCoords = SCoord.SortClockwiseOrder(tileCenter, neighbors.First.Value, source);

        triangleList.Add(keyLookup[triangleCoords[2]]);
        triangleList.Add(keyLookup[triangleCoords[1]]);
        triangleList.Add(keyLookup[triangleCoords[0]]);


        // Connext last vertex to first vertex

        // Sort vertices in clockwise order
        triangleCoords = SCoord.SortClockwiseOrder(tileCenter, neighbors.First.Value, startVertex);

        triangleList.Add(keyLookup[triangleCoords[2]]);
        triangleList.Add(keyLookup[triangleCoords[1]]);
        triangleList.Add(keyLookup[triangleCoords[0]]);


        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangleList.ToArray();
    }
}


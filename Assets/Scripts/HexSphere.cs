using System;
using System.Collections.Generic;
using UnityEngine;

public class HexSphere
{
    private Icosphere hexSphere;
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

    public Icosphere GetHexMap()
    {
        return hexSphere;
    }

    public void RenderSphere(Transform parentObject, Texture outlineHex, Texture outlinePent)
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

            if (hexSphere.GetDegree(tile) == 5)
                mr.material.SetTexture("_MainTex", outlinePent);
            else if (hexSphere.GetDegree(tile) == 6)
                mr.material.SetTexture("_MainTex", outlineHex);
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
        // Make list of normals for each vertex
        List<Vector3> normals = new List<Vector3>(neighbors.Count + 1);
        // Make list of UV coordinates for each vertex
        List<Vector2> uvLocations = new List<Vector2>(neighbors.Count + 1);

        Dictionary<SCoord, int> keyLookup = new Dictionary<SCoord, int>();

        keyLookup.Add(tileCenter, keyLookup.Count);
        uvLocations.Add(new Vector2(0.5f, 0.5f));
        vertices.Add(hexSphere.GetPoint(tileCenter));
        normals.Add(tileCenter.ToEuclidian());

        float radPerVertex = Mathf.PI * 2 / neighbors.Count;

        SCoord startVertex = neighbors.First.Value;
        SCoord source = startVertex;
        neighbors.RemoveFirst();

        // Get the order of neighbors (forward or backward just wrapping around center)
        LinkedList<SCoord> orderedNeighbors = new LinkedList<SCoord>();
        orderedNeighbors.AddLast(startVertex);

        // Calculate the teselated edges of the hexagon/pentagon
        while (neighbors.Count > 1)
        {
            // Find the next SCoord in the sequence of neighbors (neighbor to origin that is 
            // adjacent to the previous neighbor)
            LinkedListNode<SCoord> nextVertex = neighbors.First;
            while (!new List<SCoord>(hexSphere.GetNeighbors(nextVertex.Value)).Contains(source))
                nextVertex = nextVertex.Next;
            orderedNeighbors.AddLast(nextVertex.Value);

            // Save current vertex as previous vertex
            source = nextVertex.Value;
            // Remove current vertex so it is not checked again
            neighbors.Remove(nextVertex);
        }
        // Finish list of ordered neighbors
        orderedNeighbors.AddLast(neighbors.First.Value);

        foreach (SCoord n in orderedNeighbors)
            keyLookup.Add(n, keyLookup.Count);

        List<SCoord> neighborList = new List<SCoord>(orderedNeighbors);
        List<int> triangleList = new List<int>();

        for (int idx = 0; idx < neighborList.Count; idx++)
        {
            // Get the centroid of the three adjacent tiles
            SCoord vert = SCoord.GetCentroid(tileCenter, neighborList[idx], neighborList[(idx + 1) % neighborList.Count]);

            SCoord[] triangleCoords = SCoord.SortClockwiseOrder(tileCenter, 
                neighborList[idx], 
                neighborList[(idx + 1) % neighborList.Count]);

            vertices.Add(hexSphere.GetPoint(vert));
            normals.Add(vert.ToEuclidian());
            Vector2 uv = new Vector2(0.5f + Mathf.Cos(radPerVertex * idx) * 0.5f, 0.5f + Mathf.Sin(radPerVertex * idx) * 0.5f);
            uvLocations.Add(new Vector2(0.5f + Mathf.Cos(radPerVertex * idx) * 0.5f, 0.5f + Mathf.Sin(radPerVertex * idx) * 0.5f));

            triangleList.Add(keyLookup[triangleCoords[2]]);
            triangleList.Add(keyLookup[triangleCoords[1]]);
            triangleList.Add(keyLookup[triangleCoords[0]]);
        }

        // Flatten out the center of the hex so it doesn't arch out
        Vector3 flatCenter = Vector3.zero;
        for (int i = 1; i < vertices.Count; i++)
            flatCenter += vertices[i];
        flatCenter /= (vertices.Count - 1);
        vertices[0] = flatCenter;

        // assign values to mesh
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.uv = uvLocations.ToArray();
    }
}


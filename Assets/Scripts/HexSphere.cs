using System;
using System.Collections.Generic;
using UnityEditor;
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

    public GameObject GetTile(SCoord coordinate)
    {
        return tileMap[coordinate];
    }

    public Icosphere GetHexMap()
    {
        return hexSphere;
    }

    public void RenderSphere(Transform parentObject, Material outlineHex, Material outlinePent)
    {
        int sphereLayer = LayerMask.NameToLayer("Sphere"); ;

        foreach (SCoord tile in hexSphere.Coordinates)
        {
            // Setup new game object with generated mesh
            GameObject newTile = new GameObject();
            MeshFilter mf = newTile.AddComponent<MeshFilter>();
            MeshRenderer mr = newTile.AddComponent<MeshRenderer>();
            MeshCollider mc = newTile.AddComponent<MeshCollider>();
            HexIdentifier hider = newTile.AddComponent<HexIdentifier>();
            //GameObjectUtility.SetStaticEditorFlags(newTile, 
            //StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic);

            // Set the standard material shader
            mr.material = new Material(Shader.Find("Diffuse"));

            // Make the mesh and render the tile
            RenderTile(mf.mesh, tile, hexSphere);

            // Move the tile to its new position and rotation
            newTile.transform.position += parentObject.transform.position;
            newTile.transform.Rotate(parentObject.transform.eulerAngles);

            // set hierarchy relationship
            newTile.transform.SetParent(parentObject);

            // Set Name of the tile
            newTile.name = "Lat " + Mathf.Round(tile.GetTheta() * Mathf.Rad2Deg * 100) / 100 +
                " Lon " + Mathf.Round(tile.GetPhi() * Mathf.Rad2Deg * 100) / 100;

            tileMap[tile] = newTile;

            hider.center = hexSphere.GetPoint(tile);
            hider.location = tile;

            CameraHider.AddObject(hider);

            newTile.layer = sphereLayer;

            if (hexSphere.GetDegree(tile) == 5)
                mr.material = outlinePent;
            else if (hexSphere.GetDegree(tile) == 6)
                mr.material = outlineHex;

            mc.sharedMesh = mf.mesh;
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

    public static void RenderTile(Mesh mesh, SCoord tileCenter, Icosphere hexSphere)
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

        // Setup lookup table for vertices
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

            // Sort the coordinates in the correct order so the face is visible
            SCoord[] triangleCoords = SCoord.SortClockwiseOrder(tileCenter, 
                neighborList[idx], 
                neighborList[(idx + 1) % neighborList.Count]);

            // Add the vertex
            vertices.Add(hexSphere.GetPoint(vert));
            // Add the normal vector of the vertex
            normals.Add(vert.ToEuclidian());
            // Add UV coordinate for this vertex
            Vector2 uv = new Vector2(0.5f + Mathf.Cos(radPerVertex * idx) * 0.5f, 
                0.5f + Mathf.Sin(radPerVertex * idx) * 0.5f);
            uvLocations.Add(new Vector2(0.5f + Mathf.Cos(radPerVertex * idx) * 0.5f, 
                0.5f + Mathf.Sin(radPerVertex * idx) * 0.5f));

            // Add the triangles (set of three vertices)
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


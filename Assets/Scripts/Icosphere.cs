using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Icosphere that is made up of a set of equidistant veriticies that are
/// centered around a point.
/// </summary>
public class Icosphere
{
    /// <summary>
    /// Coordinate center of the icosphere. 
    /// </summary>
    private Vector3 center;

    /// <summary>
    /// Radius of the icosphere, distance from the center to any point on the surface. 
    /// </summary>
    private float radius;

    /// <summary>
    /// Set of vertices in the graph.
    /// </summary>
    private Graph<SCoord> vertices;

    /// <summary>
    /// Create an icosphere with a center and radius that contians twelve vertices
    /// </summary>
    /// <param name="center">Center of the icosphere in space</param>
    /// <param name="radius">Radius of the icosphere</param>
    public Icosphere (Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
        this.vertices = GetPointsOfIcosphere();
    }

    /// <summary>
    /// Create an icosphere with a given graph of vertices (use for subdivisions).
    /// </summary>
    /// <param name="center">Center of the icosphere in space</param>
    /// <param name="radius">Radius of the icosphere</param>
    /// <param name="vertices">Set of vertices and edges in a graph</param>
    private Icosphere(Vector3 center, float radius, Graph<SCoord> vertices)
    {
        this.center = center;
        this.radius = radius;
        this.vertices = vertices;
    }

    /// <summary>
    /// Teselates an icosphere but cutting each traingular face into four smaller traingular faces.
    /// </summary>
    /// <returns></returns>
    public Icosphere SubdivideSphere()
    {
        // List of all vertices currently in the icosphere
        List<SCoord> keys = new List<SCoord>(this.vertices.GetPoints());

        BiDirectionalEdgeComparator<SCoord> edgeComparator = new BiDirectionalEdgeComparator<SCoord>();

        // Set of all edges in the sphere (set to avoid duplicates)
        HashSet<Edge<SCoord>> edges = new HashSet<Edge<SCoord>>(edgeComparator);

        // For each vertex in the original icosphere
        foreach (SCoord point in vertices.GetPoints())
        {
            // For each edge that this point is connected to
            foreach (SCoord endpt in vertices.GetConnected(point))
            {
                // Create a structure for the edge
                Edge<SCoord> edge = new Edge<SCoord>(point, endpt);
                // Add the ege to the list of edges (Set to avoid duplicates)
                edges.Add(edge);
            }
        }
        
        // The graph of the subdivided sphere
        Graph<SCoord> subdivided = new Graph<SCoord>(vertices.GetPoints());
        // List of all the points in the subdivided sphere
        List<SCoord> cuts = new List<SCoord>(edges.Count);
        List<Edge<SCoord>> edgeList = new List<Edge<SCoord>>(edges);
        Dictionary<Edge<SCoord>, SCoord> cutLookup = new Dictionary<Edge<SCoord>, SCoord>(edgeComparator);

        // For each edge in the sphere
        foreach (Edge<SCoord> edge in edgeList)
        {
            // Get the endpoints of the edge
            SCoord[] endpts = edge.GetPoints();
            // Get the midpoint between the edge
            SCoord midpoint = SCoord.GetMidpoint(endpts[0], endpts[1]);

            cutLookup.Add(edge, midpoint);

            // Add the point to the list of cuts
            cuts.Add(midpoint);
            // Put the new cut in the new sphere
            subdivided.AddPoint(midpoint);

            // connect the subdivided point to the points it cut apart
            subdivided.Connect(midpoint, endpts[0]);
            subdivided.Connect(midpoint, endpts[1]);
        }

        
        // For each new point added, connect it to other new points added
        for (int cutIdx = 0; cutIdx < cuts.Count; cutIdx++)
        {
            // Get the point and the edge it divides
            SCoord cut = cuts[cutIdx];
            Edge<SCoord> originalEdge = edgeList[cutIdx];

            // Get the edpoints on the original edge
            SCoord[] endpts = originalEdge.GetPoints();

            // Find the other points in the graph that these two points have in common
            HashSet<SCoord> common = new HashSet<SCoord>(vertices.GetConnected(endpts[0]));
            common.IntersectWith(vertices.GetConnected(endpts[1]));
            List<SCoord> faceEdges = new List<SCoord>(common);

            // There should always be two of these points. This means for points that
            // should make up the original four edges. These plus the original edge 
            // define the two faces that the four points share in common
            Edge<SCoord>[] commonEdges = {
                new Edge<SCoord>(faceEdges[0], endpts[0]),
                new Edge<SCoord>(faceEdges[0], endpts[1]),
                new Edge<SCoord>(faceEdges[1], endpts[0]),
                new Edge<SCoord>(faceEdges[1], endpts[1])};

            // Connect the subdivisions of these other edges to this cut
            foreach (Edge<SCoord> connected in commonEdges)
                subdivided.Connect(cut, cutLookup[connected]);
        }
        
        Debug.Log(vertices);
        Debug.Log(subdivided);

        return new Icosphere(center, radius, subdivided);
    }

    /// <summary>
    /// Sets the radius of the icosphere.
    /// </summary>
    /// <param name="newRadius">Changes the radius of the sphere.</param>
    public void SetRadius(float newRadius) => radius = newRadius;


    /// <summary>
    /// Get the coordinates of an icosphere as spherical coordinates.
    /// </summary>
    /// <returns>An enumerable set of points in the spherical coordinates.</returns>
    public IEnumerable<SCoord> Coordinates => vertices.GetPoints();

    /// <summary>
    /// Gets the euclidian coordinate of a point around the center of this icosphere.
    /// </summary>
    /// <param name="coordinate">Coordinate in lattitude and lnogitude relative to the spehre</param>
    /// <returns>The point at that lattitude and longitude the radius of the icosphere
    /// away from the center of the icosphere.</returns>
    public Vector3 GetPoint(SCoord coordinate) => coordinate.ToEuclidian() * radius + center;

    /// <summary>
    /// Gets the points adjacent to a given point.
    /// </summary>
    /// <param name="coordinate">Point in the graph</param>
    /// <returns>The coordinate of that point.</returns>
    public IEnumerable<SCoord> GetNeighbors(SCoord coordinate) => vertices.GetConnected(coordinate);

    /// <summary>
    /// Check if two vertices are connected.
    /// </summary>
    /// <param name="coord1"></param>
    /// <param name="coord2"></param>
    /// <returns></returns>
    public Boolean AreConnected(SCoord coord1, SCoord coord2) => vertices.AreConnected(coord1, coord2);

    /// <summary>
    /// Gets the number of connected vertices to a given vertex.
    /// </summary>
    /// <param name="coordiante">A spherical coordinate on the icosphere that 
    /// corresponds to a vertex on the sphere.</param>
    /// <returns>The number of connected vertices to a given vertex.</returns>
    public int GetDegree(SCoord coordiante) => vertices.Degree(coordiante);

    /// <summary>
    /// Gets the points that describe the rotation around the origin of a 12 point icosphere.
    /// </summary>
    /// <returns>A graph of the points in the icosphere and their connected edges.</returns>
    private static Graph<SCoord> GetPointsOfIcosphere()
    {
        SCoord[] vertices = new SCoord[12];

        // poles of the icosphere
        vertices[0] = new SCoord(Mathf.PI / 2, 0);
        vertices[11] = new SCoord(- Mathf.PI / 2, 0);

        // Values for calculating rotation
        // lat difference for lattitude for the 10 alternating points in radians
        //  value is arctan(1/2)
        float latDifference = 0.4636476f;
        // lon difference for longitudinal distance between each point. They are equally 
        //   spaced with 10 points so each point is PI/5 radians apart
        float lonDifference = 0.6283185f;

        // points on top and bottom, alternating upper and lower sector
        for (int point = 0; point < 10; point++)
        {
            float latValue = latDifference * (point % 2 == 0 ? 1 : -1);
            float lonValue = lonDifference * point;
            vertices[point + 1] = new SCoord(latValue, lonValue);
        }

        // Make graph of points in icosaheadron
        Graph<SCoord> graph = new Graph<SCoord>(vertices);
        
        // Connect the pole to all points in the top half
        // Connect the pole to all points in the bottom half
        // Connect each point to the next two points in the sequence
        for (int point = 0; point < 10; point++)
        {
            // Connect to top pole if even, bottom pole if odd. 
            graph.Connect(point % 2 == 0 ? vertices[0] : vertices[11], vertices[point + 1]);

            // indices 1 and 11 in the sequence are the poles so avoid those.
            graph.Connect(vertices[point + 1], vertices[(point + 1) % 10 + 1]);
            graph.Connect(vertices[point + 1], vertices[(point + 2) % 10 + 1]);
        }

        // Return the graph of spherical coordinates
        return graph;
    }
}

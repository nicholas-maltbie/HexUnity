using UnityEngine;
using System;
using System.Collections.Generic;

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
    /// Get the coordinates of an icosphere as spherical coordinates.
    /// </summary>
    /// <returns>An enumerable set of points in the spherical coordinates.</returns>
    public IEnumerable<SCoord> GetCoordinates()
    {
        return vertices.GetPoints();
    }

    /// <summary>
    /// Gets the euclidian coordinate of a point around the center of this icosphere.
    /// </summary>
    /// <param name="coordinate">Coordinate in lattitude and lnogitude relative to the spehre</param>
    /// <returns>The point at that lattitude and longitude the radius of the icosphere
    /// away from the center of the icosphere.</returns>
    public Vector3 GetPoint(SCoord coordinate)
    {
        return coordinate.ToEuclidian() * radius + center;
    }

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

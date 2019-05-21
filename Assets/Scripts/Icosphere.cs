using UnityEngine;

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
    /// Create an icosphere with a center and radius that contians twelve vertices
    /// </summary>
    /// <param name="center">Center of the icosphere in space</param>
    /// <param name="radius">Radius of the icosphere</param>
    public Icosphere (Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    /// <summary>
    /// Get the twelve major points of a regular icosphere as Spherical coordinates.
    /// </summary>
    /// <returns>Returns the set of coordinates as an array of 12 spherical coordinates</returns>
    public static SCoord[] GetMajorSphericalCoordinates()
    {
        SCoord[] points = new SCoord[12];

        

        return points;
    }
}

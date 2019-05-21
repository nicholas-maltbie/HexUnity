using UnityEngine;
using System;

/// <summary>
/// Spherical Coordinate with a latitude and longitude around a sphere
/// </summary>
public class SCoord : MonoBehaviour
{
    /// <summary>
    /// Coordinates in lattidue and longitude. Measured in radians.
    /// </summary>
    private float lat, lon;

    /// <summary>
    /// Creates a Sphereical Coordinate
    /// </summary>
    /// <param name="lat">lattitude of the coordinate (in radians)</param>
    /// <param name="lon">longitude of the coordinate (in radians)</param>
    public SCoord(float lat, float lon)
    {
        this.lat = lat;
        this.lon = lon;
    }

    /// <summary>
    /// Returns a unit vector in the direction of this coordinate.
    /// </summary>
    /// <returns>A Vector3 object of length 1 in the direction of this
    /// coordinate from the center of its sphere.</returns>
    public Vector3 ToEuclidian()
    {
        return ConvertToEuclidian(this);
    }

    /// <summary>
    /// Convert a sphereical coordinate to a unit vector in the direction from the cetner of the sphere
    /// </summary>
    /// <param name="coord">Spherical cooridnate to use</param>
    /// <returns>Vector3 that points in the direction from the center of the sphere with length 1</returns>
    public static Vector3 ConvertToEuclidian(SCoord coord)
    {
        return new Vector3(Mathf.Cos(coord.lat) * Mathf.Cos(coord.lon),
            Mathf.Cos(coord.lat) * Mathf.Sin(coord.lon),
            Mathf.Sin(coord.lat));
    }
}

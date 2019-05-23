using UnityEngine;
using System;

/// <summary>
/// Spherical Coordinate with a latitude and longitude around a sphere
/// </summary>
public class SCoord
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
        while (lat < 0) lat += Mathf.PI * 2;
        while (lat > Math.PI * 2) lat -= Mathf.PI * 2;
        while (lon < 0) lon += Mathf.PI * 2;
        while (lon > Math.PI * 2) lon -= Mathf.PI * 2;

        this.lat = lat;
        this.lon = lon;

    }

    /// <summary>
    /// Get the lattitude of the coordinate.
    /// </summary>
    /// <returns>Longitude of the point as a float.</returns>
    public float GetLat()
    {
        return lat;
    }

    /// <summary>
    /// Get the longitude of the coordinate.
    /// </summary>
    /// <returns>Lattitude of the point as a float.</returns>
    public float GetLon()
    {
        return lon;
    }

    /// <summary>
    /// Gets a string representation of the lattitude and longitude of an icosphere.
    /// </summary>
    /// <returns>String of the lattitude and longitude of the point</returns>
    public override string ToString()
    {
        return "SCoord lat=" + lat.ToString() + " lon=" + lon.ToString();
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
    /// Evaluates if two SCoordinates are equivalent. Two coordinate are equal if they have the 
    /// same lattitude and longitude. 
    /// </summary>
    /// <param name="other">Other coordinate to compare to</param>
    /// <returns>True if they are the same, false otherwise.</returns>
    public override bool Equals(object other)
    {
        if (this.GetType().IsAssignableFrom(other.GetType()))
        {
            SCoord otherCoord = (SCoord)other;
            return otherCoord.lat == lat && otherCoord.lon == lon;
        }
        return false;
    }

    /// <summary>
    /// Get the hash code of a SCoord. Generated using the lattitude and longitude of a point.
    /// </summary>
    /// <returns>Arbitrary hash value for a given point. </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 683;

            hash += 59 * lat.GetHashCode();
            hash += 683 * lon.GetHashCode();

            return hash;
        }
    }

    /// <summary>
    /// Gets the angle between two coordiantes on a sphere.
    /// </summary>
    /// <param name="coord1">First coordinate on the sphere. (From)</param>
    /// <param name="coord2">Second coordinate on the sphere. (To)</param>
    /// <returns>The angle representing the direction between the two coordinates in radians.</returns>
    public static float GetAngleBetween(SCoord coord1, SCoord coord2)
    {
        return Mathf.Acos(Vector3.Dot(coord1.ToEuclidian(), coord2.ToEuclidian()));
    }

    /// <summary>
    /// Gets the rotation of an object that would be on the surface of the sphere at this coordinate.
    /// </summary>
    /// <param name="coord">coordinate on the sphere</param>
    /// <returns>Rotation of an object if it was placed on the sphere at this coordinate<returns>
    public static Vector3 GetRotation(SCoord coord)
    {
        return Quaternion.FromToRotation(Vector3.up, coord.ToEuclidian()).eulerAngles;
    }

    /// <summary>
    /// Convert a sphereical coordinate to a unit vector in the direction from the cetner of the sphere
    /// </summary>
    /// <returns>Vector3 that points in the direction from the center of the sphere with length 1</returns>
    public static Vector3 ConvertToEuclidian(SCoord coord)
    {
        return new Vector3(Mathf.Cos(coord.lat) * Mathf.Cos(coord.lon),
            Mathf.Sin(coord.lat),
            Mathf.Cos(coord.lat) * Mathf.Sin(coord.lon));
    }
}

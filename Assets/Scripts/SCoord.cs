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

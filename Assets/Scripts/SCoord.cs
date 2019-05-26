using UnityEngine;
using System;
using System.Collections.Generic;

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
        return "SCoord lat=" + (Mathf.Round(lat * 180 / Mathf.PI * 100) / 100).ToString() + 
            " lon=" + (Mathf.Round(lon * 180 / Mathf.PI * 100) / 100).ToString().ToString();
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
    public static float GetAngleBetween(SCoord coord1, SCoord coord2) => Mathf.Acos(Vector3.Dot(coord1.ToEuclidian(), coord2.ToEuclidian()));

    public static float GetBearing(SCoord coord1, SCoord coord2)
    {
        float y = Mathf.Sin(coord2.lon - coord1.lon) * Mathf.Cos(coord2.lat);
        float x = Mathf.Cos(coord1.lat) * Mathf.Sin(coord2.lat) -
                Mathf.Sin(coord1.lat) * Mathf.Cos(coord2.lat) * Mathf.Cos(coord2.lon - coord1.lon);
        return Mathf.Atan2(y, x);
    }

    public static SCoord GetPointAlongBearing(SCoord start, float bearing, float angularDistance, float radius)
    {
        float lat2 = Mathf.Asin(Mathf.Sin(start.lat) * Mathf.Cos(angularDistance / radius) +
                    Mathf.Cos(start.lat) * Mathf.Sin(angularDistance / radius) * Mathf.Cos(bearing));
        float lon2 = start.lon + Mathf.Atan2(Mathf.Sin(bearing) * Mathf.Sin(angularDistance / radius) * Mathf.Cos(start.lat),
                                 Mathf.Cos(angularDistance / radius) - Mathf.Sin(start.lat) * Mathf.Sin(lat2));
        return new SCoord(lat2, lon2);
    }

    public static SCoord GetIntermediatePoint(SCoord coord1, SCoord coord2, float fraction)
    {
        return GetIntermediatePoint(coord1, coord2, fraction, GetAngleBetween(coord1, coord2));
    }

    public static SCoord GetIntermediatePoint(SCoord coord1, SCoord coord2, float fraction, float delta)
    {
        float a = Mathf.Sin((1 - fraction) * delta) / Mathf.Sin(delta);
        float b = Mathf.Sin(fraction * delta) / Mathf.Sin(delta);

        float x = a * Mathf.Cos(coord1.GetLat()) * Mathf.Cos(coord1.GetLon()) + b * Mathf.Cos(coord2.GetLat()) * Mathf.Cos(coord2.GetLon());
        float y = a * Mathf.Cos(coord1.GetLat()) * Mathf.Sin(coord1.GetLon()) + b * Mathf.Cos(coord2.GetLat()) * Mathf.Sin(coord2.GetLon());
        float z = a * Mathf.Sin(coord1.GetLat()) + b * Mathf.Sin(coord2.GetLat());

        return new SCoord(Mathf.Atan2(z, Mathf.Sqrt(x * x + y * y)), Mathf.Atan2(y, x));
    }

    /// <summary>
    /// Gets the midpoint between two spherical coordinates. 
    /// </summary>
    /// <param name="coord1">First coordinate</param>
    /// <param name="coord2">Second coordinate</param>
    /// <returns>The point that lies between the two points along the great circle between them.</returns>
    public static SCoord GetMidpoint(SCoord coord1, SCoord coord2)
    {
        float lat1 = coord1.lat;
        float lon1 = coord1.lon;
        float lat2 = coord2.lat;
        float lon2 = coord2.lon;

        float dLon = (lon2 - lon1 + Mathf.PI * 2) % (Mathf.PI * 2);


        float Bx = Mathf.Cos(lat2) * Mathf.Cos(dLon);
        float By = Mathf.Cos(lat2) * Mathf.Sin(dLon);
        float lat3 = Mathf.Atan2(Mathf.Sin(lat1) + Mathf.Sin(lat2), Mathf.Sqrt((Mathf.Cos(lat1) + Bx) * (Mathf.Cos(lat1) + Bx) + By * By));
        float lon3 = lon1 + Mathf.Atan2(By, Mathf.Cos(lat1) + Bx);

        return new SCoord(lat3, lon3);
    }

    /// <summary>
    /// Gets the median point of a set of spherical coordinates
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static SCoord GetCentroid(params SCoord[] coords)
    {
        Vector3 center = Vector3.zero;
        foreach (SCoord coord in coords)
            center += coord.ToEuclidian();
        return SCoord.ConvertToSCoord(center.normalized);
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

    /// <summary>
    /// Converts a 3d coordinate to an SCoord with the origin (0,0,0) as the center of the sphere
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static SCoord ConvertToSCoord(Vector3 vector)
    {
        if (vector.x == 0)
            vector.x = Mathf.Epsilon;
        float outLon = Mathf.Atan(vector.z / vector.x);
        if (vector.x < 0)
            outLon += Mathf.PI;
        float outLat = Mathf.Asin(vector.y);

        return new SCoord(outLat, outLon);
    }

    /// <summary>
    ///  Sorts a set of three spherical coordinates in clockwise order
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <param name="s3"></param>
    /// <returns>The ordered arary of vertices in clockwise order</returns>
    public static SCoord[] SortClockwiseOrder(SCoord s1, SCoord s2, SCoord s3)
    {
        Vector3 v1 = s1.ToEuclidian();
        Vector3 v2 = s2.ToEuclidian();
        Vector3 v3 = s3.ToEuclidian();

        Vector3 centroid = (v1 + v2 + v3).normalized;
        Vector3 n = Vector3.Cross(v2 - v1, v3 - v1);

        float w = Vector3.Dot(n, v2 - centroid);

        if (w > 0)
        {
            return new SCoord[] { s1, s2, s3 };
        }
        return new SCoord[] { s3, s2, s1 };
    }

    /// <summary>
    ///  Sorts a set of three spherical coordinates in anticlockwise order
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <param name="s3"></param>
    /// <returns>The ordered arary of vertices in anti clockwise order</returns>
    public static SCoord[] SortAntiClockwiseOrder(SCoord s1, SCoord s2, SCoord s3)
    {
        List<SCoord> coords = new List<SCoord>(SortClockwiseOrder(s1, s2, s3));
        coords.Reverse();
        return coords.ToArray();
    }
}

/// <summary>
/// Compare two SCoordinates sorting by lattitude then longitude.
/// </summary>
public class SCoordComparatorLat : IComparer<SCoord>
{
    /// <summary>
    /// Compares two coordinates. Uses lattitude before longitude. This is mostly
    /// and arbitrary but consistant sorting method.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>
    /// 1 if x's lattitude is greater than y's
    /// -1 if x's lattitude is less than y's
    /// if x's lattitude and y's lattitude are equal
    /// 1 if x's longitude is greater than y's longitude
    /// -1 if y's longitude is less than y's longitude
    /// 0 if x's longitude equals y's longitude
    /// </returns>
    public int Compare(SCoord x, SCoord y)
    {
        if (x.GetLat() > y.GetLat())
        {
            return 1;
        }
        if (x.GetLat() < y.GetLat())
        {
            return -1;
        }

        if (x.GetLon() > y.GetLon())
        {
            return 1;
        }
        if (x.GetLon() < y.GetLon())
        {
            return -1;
        }

        return 0;
    }
}

/// <summary>
/// Compare two SCoordinates sorting by longitude then lattitude.
/// </summary>
public class SCoordComparatorLon : IComparer<SCoord>
{
    /// <summary>
    /// Compares two coordinates. Uses longitude before lattitude. This is mostly
    /// and arbitrary but consistant sorting method.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>
    /// 1 if x's longitude is greater than y's
    /// -1 if x's longitude is less than y's
    /// if x's longitude and y's longitude are equal
    /// 1 if x's lattitude is greater than y's lattitude
    /// -1 if y's lattitude is less than y's lattitude
    /// 0 if x's lattitude equals y's lattitude
    /// </returns>
    public int Compare(SCoord x, SCoord y)
    {
        if (x.GetLon() > y.GetLon())
        {
            return 1;
        }
        if (x.GetLon() < y.GetLon())
        {
            return -1;
        }

        if (x.GetLat() > y.GetLat())
        {
            return 1;
        }
        if (x.GetLat() < y.GetLat())
        {
            return -1;
        }

        return 0;
    }
}

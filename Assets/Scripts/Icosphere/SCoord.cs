using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Spherical Coordinate with a thetaitude and phigitude around a sphere
/// </summary>
public class SCoord
{
    /// <summary>
    /// Coordinates of theta and phi. Measured in radians.
    /// </summary>
    private float theta, phi;

    /// <summary>
    /// Creates a Sphereical Coordinate
    /// </summary>
    /// <param name="theta">polar angle  of the coordinate (in radians)</param>
    /// <param name="phi">azimuthal angle of the coordinate (in radians)</param>
    public SCoord(float theta, float phi)
    {
        this.theta = theta;
        this.phi = phi;

    }

    /// <summary>
    /// Get the polar angle of the coordinate.
    /// </summary>
    /// <returns>polar angle (theta) of the point as a float.</returns>
    public float GetTheta()
    {
        return theta;
    }

    /// <summary>
    /// Get the azimuthal angle of the coordinate.
    /// </summary>
    /// <returns>azimuthal angle (phi) of the point as a float.</returns>
    public float GetPhi()
    {
        return phi;
    }

    /// <summary>
    /// Gets a string representation of the polar angle and azimuthal angle of an icosphere.
    /// </summary>
    /// <returns>String of the thetatitude and phigitude of the point</returns>
    public override string ToString()
    {
        float t = theta - Mathf.PI * 2 * (theta / Mathf.PI * 2);
        float p = phi - Mathf.PI * 2 * (phi / Mathf.PI * 2);

        return "SCoord theta=" + (Mathf.Round(t * 180 / Mathf.PI * 100) / 100).ToString() + 
            " phi=" + (Mathf.Round(p * 180 / Mathf.PI * 100) / 100).ToString().ToString();
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
    /// same polar angle and azimuthal angle. 
    /// </summary>
    /// <param name="other">Other coordinate to compare to</param>
    /// <returns>True if they are the same, false otherwise.</returns>
    public override bool Equals(object other)
    {
        if (this.GetType().IsAssignableFrom(other.GetType()))
        {
            SCoord otherCoord = (SCoord)other;
            return otherCoord.theta == theta && otherCoord.phi == phi;
        }
        return false;
    }

    /// <summary>
    /// Get the hash code of a SCoord. Generated using the thetatitude and phigitude of a point.
    /// </summary>
    /// <returns>Arbitrary hash value for a given point. </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 683;

            hash += 59 * theta.GetHashCode();
            hash += 683 * phi.GetHashCode();

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
        float y = Mathf.Sin(coord2.phi - coord1.phi) * Mathf.Cos(coord2.theta);
        float x = Mathf.Cos(coord1.theta) * Mathf.Sin(coord2.theta) -
                Mathf.Sin(coord1.theta) * Mathf.Cos(coord2.theta) * Mathf.Cos(coord2.phi - coord1.phi);
        return Mathf.Atan2(y, x);
    }

    public static SCoord GetPointAphigBearing(SCoord start, float bearing, float angularDistance, float radius)
    {
        float theta2 = Mathf.Asin(Mathf.Sin(start.theta) * Mathf.Cos(angularDistance / radius) +
                    Mathf.Cos(start.theta) * Mathf.Sin(angularDistance / radius) * Mathf.Cos(bearing));
        float phi2 = start.phi + Mathf.Atan2(Mathf.Sin(bearing) * Mathf.Sin(angularDistance / radius) * Mathf.Cos(start.theta),
                                 Mathf.Cos(angularDistance / radius) - Mathf.Sin(start.theta) * Mathf.Sin(theta2));
        return new SCoord(theta2, phi2);
    }

    public static SCoord GetIntermediatePoint(SCoord coord1, SCoord coord2, float fraction)
    {
        return GetIntermediatePoint(coord1, coord2, fraction, GetAngleBetween(coord1, coord2));
    }

    public static SCoord GetIntermediatePoint(SCoord coord1, SCoord coord2, float fraction, float delta)
    {
        float a = Mathf.Sin((1 - fraction) * delta) / Mathf.Sin(delta);
        float b = Mathf.Sin(fraction * delta) / Mathf.Sin(delta);

        float x = a * Mathf.Cos(coord1.GetTheta()) * Mathf.Cos(coord1.GetPhi()) + b * Mathf.Cos(coord2.GetTheta()) * Mathf.Cos(coord2.GetPhi());
        float y = a * Mathf.Cos(coord1.GetTheta()) * Mathf.Sin(coord1.GetPhi()) + b * Mathf.Cos(coord2.GetTheta()) * Mathf.Sin(coord2.GetPhi());
        float z = a * Mathf.Sin(coord1.GetTheta()) + b * Mathf.Sin(coord2.GetTheta());

        return new SCoord(Mathf.Atan2(z, Mathf.Sqrt(x * x + y * y)), Mathf.Atan2(y, x));
    }

    /// <summary>
    /// Gets the midpoint between two spherical coordinates. 
    /// </summary>
    /// <param name="coord1">First coordinate</param>
    /// <param name="coord2">Second coordinate</param>
    /// <returns>The point that lies between the two points aphig the great circle between them.</returns>
    public static SCoord GetMidpoint(SCoord coord1, SCoord coord2)
    {
        float theta1 = coord1.theta;
        float phi1 = coord1.phi;
        float theta2 = coord2.theta;
        float phi2 = coord2.phi;

        float dphi = (phi2 - phi1 + Mathf.PI * 2) % (Mathf.PI * 2);


        float Bx = Mathf.Cos(theta2) * Mathf.Cos(dphi);
        float By = Mathf.Cos(theta2) * Mathf.Sin(dphi);
        float theta3 = Mathf.Atan2(Mathf.Sin(theta1) + Mathf.Sin(theta2), Mathf.Sqrt((Mathf.Cos(theta1) + Bx) * (Mathf.Cos(theta1) + Bx) + By * By));
        float phi3 = phi1 + Mathf.Atan2(By, Mathf.Cos(theta1) + Bx);

        return new SCoord(theta3, phi3);
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
        return new Vector3(Mathf.Cos(coord.theta) * Mathf.Cos(coord.phi),
            Mathf.Sin(coord.theta),
            Mathf.Cos(coord.theta) * Mathf.Sin(coord.phi));
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
        float outphi = Mathf.Atan(vector.z / vector.x);
        if (vector.x < 0)
            outphi += Mathf.PI;
        float outtheta = Mathf.Asin(vector.y);

        return new SCoord(outtheta, outphi);
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
/// Compare two SCoordinates sorting by theta then phi.
/// </summary>
public class SCoordComparatorTheta : IComparer<SCoord>
{
    /// <summary>
    /// Compares two coordinates. Uses theta before phi. This is mostly
    /// and arbitrary but consistant sorting method.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>
    /// 1 if x's theta is greater than y's
    /// -1 if x's theta is less than y's
    /// if x's theta and y's thetatitude are equal
    /// 1 if x's phi is greater than y's phi
    /// -1 if y's phi is less than y's phi
    /// 0 if x's phi equals y's phi
    /// </returns>
    public int Compare(SCoord x, SCoord y)
    {
        if (x.GetTheta() > y.GetTheta())
        {
            return 1;
        }
        if (x.GetTheta() < y.GetTheta())
        {
            return -1;
        }

        if (x.GetPhi() > y.GetPhi())
        {
            return 1;
        }
        if (x.GetPhi() < y.GetPhi())
        {
            return -1;
        }

        return 0;
    }
}

/// <summary>
/// Compare two SCoordinates sorting by phi then theta.
/// </summary>
public class SCoordComparatorPhi : IComparer<SCoord>
{
    /// <summary>
    /// Compares two coordinates. Uses phi before theta. This is mostly
    /// and arbitrary but consistant sorting method.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>
    /// 1 if x's phi is greater than y's
    /// -1 if x's phi is less than y's
    /// if x's phi and y's phigitude are equal
    /// 1 if x's theta is greater than y's thetatitude
    /// -1 if y's theta is less than y's thetatitude
    /// 0 if x's theta equals y's thetatitude
    /// </returns>
    public int Compare(SCoord x, SCoord y)
    {
        if (x.GetPhi() > y.GetPhi())
        {
            return 1;
        }
        if (x.GetPhi() < y.GetPhi())
        {
            return -1;
        }

        if (x.GetTheta() > y.GetTheta())
        {
            return 1;
        }
        if (x.GetTheta() < y.GetTheta())
        {
            return -1;
        }

        return 0;
    }
}

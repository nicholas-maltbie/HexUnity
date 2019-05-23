using System;
using System.Collections.Generic;

/// <summary>
/// Edge is a a set of two of something (Can be any class)
/// </summary>
/// <typeparam name="T">Type of objects in this edge</typeparam>
public class Edge<T>
{
    /// <summary>
    /// Two objects in the edge.
    /// </summary>
    private T point1, point2;

    /// <summary>
    /// Create an edge of two objects.
    /// </summary>
    /// <param name="point1">Object 1 in the edge</param>
    /// <param name="point2">Object 2 in the edge</param>
	public Edge(T point1, T point2)
	{
        this.point1 = point1;
        this.point2 = point2;
	}

    /// <summary>
    /// Checkts if a given object is a member of this edge.
    /// </summary>
    /// <param name="obj">Checks if an object is a member of this edge.</param>
    /// <returns></returns>
    public Boolean Member(T obj) => point1.Equals(obj) || point2.Equals(obj);

    /// <summary>
    /// Get the set of points in the edge
    /// </summary>
    /// <returns>An array of the two poitns in the edge in order of point1, point2.</returns>
    public T[] GetPoints() => new T[] { point1, point2 };

    /// <summary>
    /// Reverses an edge changing the order of points.
    /// </summary>
    /// <returns>A new edge with the order of points reversed.</returns>
    public Edge<T> Reverse() => new Edge<T>(point2, point1);

    /// <summary>
    /// Checks if this edge is equivalent to another edge regardless of order of points.
    /// </summary>
    /// <param name="other">Another edge</param>
    /// <returns>Compares if two edges are equivalent regardless of order.</returns>
    public Boolean BiDirectionalEquals(Edge<T> other) => BiDirectionalEquals(this, other);

    /// <summary>
    /// Compares if two edges are equal (Have the same start and end).
    /// </summary>
    /// <param name="other">Another edge.</param>
    /// <returns>Returns whether two edges are equivalent.</returns>
    public override Boolean Equals(object other)
    {
        if (this.GetType().IsAssignableFrom(other.GetType()))
        {
            Edge<T> otherEdge = (Edge<T>)other;
            return point1.Equals(otherEdge.point1) && point2.Equals(otherEdge.point2);
        }
        return false;
    }

    /// <summary>
    /// Get the hash code of a Edge. Generated using the endpoints of the edge.
    /// </summary>
    /// <returns>Arbitrary hash value for a given Edge. </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 349;

            hash += 269 * point1.GetHashCode();
            hash += 991 * point2.GetHashCode();

            return hash;
        }
    }

    public override string ToString()
    {
        return "Edge " + point1.ToString() + " " + point2.ToString();
    }

    /// <summary>
    /// Get the hash code of a Edge. Generated using the endpoints of the edge. As 
    /// if the point is in either order.
    /// </summary>
    /// <returns>Arbitrary hash value for a given Edge. </returns>
    public static int GetBidirectionalHash(Edge<T> edge)
    {
        unchecked
        {
            int hash = 349;

            int v1 = edge.point1.GetHashCode();
            int v2 = edge.point2.GetHashCode();

            int vMin = Math.Min(v1, v2);
            int vMax = Math.Max(v1, v2);

            hash += 269 * vMin;
            hash += 991 * vMax;

            return hash;
        }
    }

    /// <summary>
    /// Checks if two edges are equivalent no matter the order.
    /// </summary>
    /// <param name="edge1">First edge</param>
    /// <param name="edge2">Second edge</param>
    /// <returns>True if the edges have the same endpoints regardless of order.</returns>
    public static Boolean BiDirectionalEquals(Edge<T> edge1, Edge<T> edge2) => edge1.Equals(edge2) || edge1.Reverse().Equals(edge2);
}

/// <summary>
/// Comparator for edges that 
/// </summary>
/// <typeparam name="T">Type of edge</typeparam>
public class BiDirectionalEdgeComparator<T> : IEqualityComparer<Edge<T>>
{
    /// <summary>
    /// Check if two edges are equal regardless of order, uses BiDirectionalEquals.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(Edge<T> x, Edge<T> y)
    {
        return x.BiDirectionalEquals(y);
    }

    /// <summary>
    /// Generates a hash for an object regardless of order.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(Edge<T> obj)
    {
        return Edge<T>.GetBidirectionalHash(obj);
    }
}

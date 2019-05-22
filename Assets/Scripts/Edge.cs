using System;

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
    public Boolean Member(T obj)
    {
        return point1.Equals(obj) || point2.Equals(obj);
    }

    /// <summary>
    /// Get the set of points in the edge
    /// </summary>
    /// <returns>An array of the two poitns in the edge in order of point1, point2.</returns>
    public T[] GetPoints() => new T[] { point1, point2 };
}

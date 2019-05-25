using System.Collections.Generic;

/// <summary>
/// A Face is a collection of vertices
/// </summary>
/// <typeparam name="E">Type of vertices in the set</typeparam>
public class Face<E>
{
    /// <summary>
    /// Vertices in the face
    /// </summary>
    private List<E> vertices;

    /// <summary>
    /// Create a face from a set of vertices
    /// </summary>
    /// <param name="vertices">ordered vertices in the face</param>
    public Face(IEnumerable<E> vertices)
    {
        this.vertices = new List<E>(vertices);
    }

    /// <summary>
    /// Get vertices of this face
    /// </summary>
    /// <returns>An duplicate enumerable list of vertices</returns>
    public IEnumerable<E> GetVertices()
    {
        return new List<E>(vertices);
    }

    /// <summary>
    /// Compares two faces if they are the same (order matters)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (this.GetType().IsAssignableFrom(obj.GetType()))
        {
            Face<E> other = (Face<E>)obj;
            return vertices.Equals(other.vertices);
        }
        return false;
    }

    /// <summary>
    /// Gets the hash of this face (hash of all its vertices, order matters)
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 19;
            foreach (E var in vertices)
            {
                hash = hash * 31 + var.GetHashCode();
            }
            return hash;
        }
    }
}

/// <summary>
/// Comparator for faces without regard for the order of vertices in the face
/// </summary>
/// <typeparam name="E"></typeparam>
public class UnDirectedFaceComparer<E> : IEqualityComparer<Face<E>>
{
    private IComparer<E> elemComp = null;

    /// <summary>
    /// Makes an undirected face comparator
    /// </summary>
    /// <param name="elemComp">Sorting method for vertices</param>
    public UnDirectedFaceComparer(IComparer<E> elemComp)
    {
        this.elemComp = elemComp;
    }

    /// <summary>
    /// Checks if two faces are equal regardless of order.
    /// </summary>
    /// <param name="x">First Face</param>
    /// <param name="y">Second Face</param>
    /// <returns>True if they contain the same set of vertices (regardless of order)</returns>
    public bool Equals(Face<E> x, Face<E> y) => new HashSet<E>(x.GetVertices()).SetEquals(y.GetVertices());

    /// <summary>
    /// Gets the hash code of the face. Will first order vertices using comaprator then get the hash.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(Face<E> obj) {
        List<E> verts = new List<E>(obj.GetVertices());
        if (elemComp != null)
        {
            verts.Sort(elemComp);
        }
        unchecked
        {
            int hash = 19;
            foreach (E var in verts)
            {
                hash = hash * 31 + var.GetHashCode();
            }
            return hash;
        }
    }
}

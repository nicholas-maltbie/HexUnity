using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A graph is a collection of points and edges. This graph uses a 
/// Dictionary of each point to a linked list of adjacent points to 
/// represent the graph in memory.
/// </summary>
/// <typeparam name="T">Type of points in a graph.</typeparam>

namespace Assets.Scripts.Graph
{
    public class Graph<T>
    {
        /// <summary>
        /// Graph of points
        /// </summary>
        private Dictionary<T, LinkedList<T>> graph;

        /// <summary>
        /// Create a graph containing a set of points
        /// </summary>
        /// <param name="points">Points that the graph is composed of.</param>
        public Graph(IEnumerable<T> points)
        {
            graph = new Dictionary<T, LinkedList<T>>();
            foreach (T point in points)
            {
                graph.Add(point, new LinkedList<T>());
            }
        }

        /// <summary>
        /// Add a point to a graph. Must be done before actions can be taken with the given point.
        /// </summary>
        /// <param name="point">Point to add to a graph.</param>
        /// <returns>Returns false if the point is already in the graph and true otherwise.</returns>
        public Boolean AddPoint(T point)
        {
            if (Contains(point))
                return false;
            graph.Add(point, new LinkedList<T>());
            return true;
        }

        /// <summary>
        /// Add a bi-directional edge between two points in a graph.
        /// </summary>
        /// <param name="point1">A point in the edge.</param>
        /// <param name="point2">A point in the edge</param>
        /// <returns>Returns false if the edges were already connected, true if 
        /// a new connection is added to the edge.</returns>
        public Boolean Connect(T point1, T point2)
        {
            if (AreConnected(point1, point2) && AreConnected(point2, point1))
                return false;
            if (!AreConnected(point1, point2))
                graph[point1].AddLast(point2);
            if (!AreConnected(point2, point1))
                graph[point2].AddLast(point1);
            return true;
        }

        /// <summary>
        /// Checks if a point is contained in the graph.
        /// </summary>
        /// <param name="point">Point to search for.</param>
        /// <returns>Returns ture if the point is in the graph, false otherwise.</returns>
        public Boolean Contains(T point)
        {
            return graph.ContainsKey(point);
        }

        /// <summary>
        /// Finds the degree of a point in a graph (number of points that 
        /// the given point is connected to).
        /// </summary>
        /// <param name="point">Point to search for in the graph.</param>
        /// <returns>Number of points that are adjacent to the point.</returns>
        public int Degree(T point)
        {
            return graph[point].Count;
        }

        /// <summary>
        /// Check if two points are connected in a graph. 
        /// </summary>
        /// <param name="point1">First point in the edge.</param>
        /// <param name="point2">Second point in the edge.</param>
        /// <returns>Returns true if the points are connected, false otherwise.</returns>
        public Boolean AreConnected(T point1, T point2)
        {
            return graph[point1].Contains(point2);
        }

        /// <summary>
        /// Gets an enumerable object representing the points adjacent
        /// to a given poitn in a graph. The order of the points is arbitrary.
        /// </summary>
        /// <param name="point">A given point in the graph.</param>
        /// <returns>An enumerable collection of objects connected to the point.</returns>
        public IEnumerable<T> GetConnected(T point)
        {
            return graph[point];
        }

        public override string ToString()
        {
            List<String> lines = new List<String>();
            foreach (T key in graph.Keys)
            {
                lines.Add(key + " : " + String.Join(", ", GetConnected(key)));
            }
            return String.Join("\n", lines);
        }

        /// <summary>
        /// Gets the set of points that this graph contains
        /// </summary>
        /// <returns>Enumerable object of points. </returns>
        public IEnumerable<T> GetPoints() => graph.Keys;
    }
}

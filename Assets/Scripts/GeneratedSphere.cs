using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GeneratedSphere : MonoBehaviour
{
    public GameObject hexagonPrefab, pentagonPrefab;
    public float radius = 2.0f;
    public int subdivsions = 1;
    private Icosphere sphere;
    private Dictionary<SCoord, GameObject> tiles = new Dictionary<SCoord, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Make the icosphere
        sphere = new Icosphere(transform.position, radius);

        for (int sub = 0; sub < subdivsions; sub++)
            sphere = sphere.SubdivideSphere();

        // Make the faces of the icosphere
        foreach (SCoord coord in sphere.Coordinates)
        {
            // Get the 3d coordinate of the sphere
            Vector3 point = sphere.GetPoint(coord);

            IEnumerator<SCoord> neighbors = sphere.GetNeighbors(coord).GetEnumerator();
            neighbors.MoveNext();
            SCoord neighbor = neighbors.Current;
            do
            {
                neighbors.MoveNext();
            }
            while (!(new List<SCoord>(sphere.GetNeighbors(neighbors.Current)).Contains(neighbor)));
            SCoord neighbor2 = neighbors.Current;
            SCoord target = SCoord.GetMidpoint(neighbor, neighbor2);


            // Get the rotation of the face to make it tangent to the sphere
            Vector3 rotation = SCoord.GetRotation(coord);

            // Get the kind of object
            GameObject newObj = null;
            int degree = sphere.GetDegree(coord);
            if (degree == 5)
                newObj = Instantiate(pentagonPrefab);
            else
                newObj = Instantiate(hexagonPrefab);

            // Place and rotate object tangent to the sphere
            newObj.transform.eulerAngles = rotation;
            newObj.transform.position = point;
            newObj.transform.SetParent(this.transform);

            // Save current position of the face's prime vertex
            Vector3 currentPosition = Vector3.ProjectOnPlane(coord.ToEuclidian() - newObj.transform.right, newObj.transform.up.normalized);

            // Project target vertex using one of the neighbors
            Vector3 proj = Vector3.ProjectOnPlane(coord.ToEuclidian() - target.ToEuclidian(), newObj.transform.up.normalized);
            float angle = angle = Vector3.Angle(currentPosition, proj);

            // Rotate the face to be line up correclty
            newObj.transform.Rotate(0, angle, 0, Space.Self);

            tiles.Add(coord, newObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SCoord coord in sphere.Coordinates)
        {
            IEnumerator<SCoord> neighbors = sphere.GetNeighbors(coord).GetEnumerator();
            neighbors.MoveNext();
            SCoord neighbor = neighbors.Current;
            do
            {
                neighbors.MoveNext();
            }
            while (!(new List<SCoord>(sphere.GetNeighbors(neighbors.Current)).Contains(neighbor)));
            SCoord neighbor2 = neighbors.Current;
            SCoord target = SCoord.GetMidpoint(neighbor, neighbor2);

            // Save current position of the face's prime vertex
            Vector3 currentPosition = Vector3.ProjectOnPlane(coord.ToEuclidian() - tiles[coord].transform.right, tiles[coord].transform.up.normalized);

            // Project target vertex using one of the neighbors
            Vector3 proj = Vector3.ProjectOnPlane(coord.ToEuclidian() - target.ToEuclidian(), tiles[coord].transform.up.normalized);
            float angle = angle = Vector3.Angle(currentPosition, proj);
            Debug.DrawRay(sphere.GetPoint(coord), proj, Color.green);
            Debug.DrawRay(sphere.GetPoint(coord), tiles[coord].transform.up, Color.yellow);
            Debug.DrawRay(sphere.GetPoint(coord), tiles[coord].transform.right, Color.red);

            foreach (SCoord other in sphere.GetNeighbors(coord))
            {
                Debug.DrawLine(sphere.GetPoint(coord), sphere.GetPoint(other), Color.blue);
            }
        }
    }
}

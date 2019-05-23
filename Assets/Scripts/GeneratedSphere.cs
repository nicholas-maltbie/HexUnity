using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GeneratedSphere : MonoBehaviour
{
    public GameObject hexagonPrefab, pentagonPrefab;
    public float radius = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Make the icosphere
        Icosphere sphere = new Icosphere(transform.position, radius);

        // Make the faces of the icosphere
        foreach (SCoord coord in sphere.GetCoordinates())
        {
            // Get the 3d coordinate of the sphere
            Vector3 point = sphere.GetPoint(coord);

            IEnumerator<SCoord> neighbors = sphere.GetNeighbors(coord).GetEnumerator();
            neighbors.MoveNext();
            SCoord neighbor = neighbors.Current;

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
            Vector3 currentPosition = newObj.transform.right;

            // Project target vertex using one of the neighbors
            Vector3 proj = Vector3.ProjectOnPlane(point - neighbor.ToEuclidian(), newObj.transform.up.normalized);
            float angle = angle = Vector3.Angle(currentPosition, proj);

            // Rotate the face to be line up correclty
            newObj.transform.Rotate(0, angle, 0, Space.Self);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

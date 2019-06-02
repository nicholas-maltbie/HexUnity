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


            // Rotate the face to be line up correclty
            IEnumerator<SCoord> neighbors = sphere.GetNeighbors(coord).GetEnumerator();
            neighbors.MoveNext();
            SCoord neighbor = neighbors.Current;

            Vector3 targetVec = Vector3.ProjectOnPlane(neighbor.ToEuclidian() - coord.ToEuclidian(), newObj.transform.up).normalized;

            float angle = Mathf.Acos(Vector3.Dot(newObj.transform.right.normalized, targetVec));
            if (float.IsNaN(angle))
            {
                angle = 180;
            }
            Vector3 cross = Vector3.Cross(newObj.transform.right.normalized, targetVec);
            
            if (Vector3.Dot(newObj.transform.up.normalized, cross) < 0)
            {
                angle *= -1;
            }

            angle *= 180 / Mathf.PI;

            Debug.Log(angle);

            newObj.transform.Rotate(0, angle, 0, Space.Self);
            tiles.Add(coord, newObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*foreach (SCoord coord in sphere.Coordinates)
        {
            IEnumerator<SCoord> neighbors = sphere.GetNeighbors(coord).GetEnumerator();
            neighbors.MoveNext();
            SCoord neighbor = neighbors.Current;

            Vector3 targetVec = Vector3.ProjectOnPlane(neighbor.ToEuclidian() - coord.ToEuclidian(), tiles[coord].transform.up).normalized;

            Debug.DrawRay(sphere.GetPoint(coord), targetVec, Color.green);
            Debug.DrawRay(sphere.GetPoint(coord), tiles[coord].transform.up.normalized, Color.yellow);
            Debug.DrawRay(sphere.GetPoint(coord), tiles[coord].transform.right.normalized, Color.red);

            foreach (SCoord other in sphere.GetNeighbors(coord))
            {
                Debug.DrawLine(sphere.GetPoint(coord), sphere.GetPoint(other), Color.blue);
            }
        }*/
    }
}

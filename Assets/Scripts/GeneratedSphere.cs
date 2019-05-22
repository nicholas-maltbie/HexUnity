using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GeneratedSphere : MonoBehaviour
{
    public GameObject hexPrefab;

    // Start is called before the first frame update
    void Start()
    {

        Icosphere sphere = new Icosphere(Vector3.zero, 1);
        foreach (SCoord coord in sphere.GetCoordinates())
        {
            Vector3 point = sphere.GetPoint(coord);
            GameObject newObj = Instantiate(hexPrefab);
            newObj.transform.position = point;
            print(newObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

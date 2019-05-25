﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSphere : MonoBehaviour
{
    private HexSphere hexSphere;
    public int subdivisions;
    public float edgeLength;


    // Start is called before the first frame update
    void Start()
    {
        hexSphere = new HexSphere(subdivisions, transform.position, edgeLength);

        hexSphere.RenderSphere(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

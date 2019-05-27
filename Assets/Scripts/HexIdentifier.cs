using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class HexIdentifier : MonoBehaviour
{
    public Vector3 center;
    public SCoord location;
    Renderer m_Renderer;

    // Use this for initialization
    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSphere : MonoBehaviour
{
    private HexSphere hexSphere;
    public int subdivisions;
    public Material hexagonOutline, pentagonOutline;
    public float edgeLength;

    // Start is called before the first frame update
    void Start()
    {
        hexSphere = new HexSphere(subdivisions, transform.position, edgeLength);

        hexSphere.RenderSphere(transform, hexagonOutline, pentagonOutline);
        CameraHider.sphere = hexSphere;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HexSphere GetSphere()
    {
        return hexSphere;
    }
}

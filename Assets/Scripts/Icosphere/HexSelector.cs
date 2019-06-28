using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelector : MonoBehaviour
{
    /// <summary>
    /// Layer of the sphere for layer masks
    /// </summary>
    int sphereLayer;
    public Texture hexHighlight, pentHighlight;
    public GameSphere sphere;

    private SCoord selected;

    private GameObject outlineHighlight;

    // Start is called before the first frame update
    void Start()
    {

    }

    public SCoord GetSelected()
    {
        return selected;
    }

    // Update is called once per frame
    void Update()
    {
        HexSphere hexSphere = sphere.GetSphere();
        Icosphere gameSphere = hexSphere.GetHexMap();

        if (gameSphere == null)
            return;

        if (Input.GetButtonDown("Select"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Sphere")))
            {
                if (outlineHighlight != null)
                    GameObject.Destroy(outlineHighlight);

                GameObject obj = hit.collider.gameObject;

                HexIdentifier hex = obj.GetComponent<HexIdentifier>();

                if (hex != null)
                {
                    selected = hex.location;

                    float rad = gameSphere.Radius;

                    gameSphere.SetRadius(rad + 0.001f);

                    outlineHighlight = new GameObject();

                    outlineHighlight.transform.position += sphere.transform.position;
                    outlineHighlight.transform.Rotate(sphere.transform.eulerAngles);

                    outlineHighlight.name = "Outline Highlight";
                    MeshFilter mf = outlineHighlight.AddComponent<MeshFilter>();
                    MeshRenderer mr = outlineHighlight.AddComponent<MeshRenderer>();
                    mr.material = new Material(Shader.Find("Transparent/Diffuse"));

                    HexSphere.RenderTile(mf.mesh, selected, gameSphere);

                    if (new List<SCoord>(gameSphere.GetNeighbors(selected)).Count == 5)
                        mr.material.SetTexture("_MainTex", pentHighlight);
                    else
                        mr.material.SetTexture("_MainTex", hexHighlight);

                    gameSphere.SetRadius(rad);

                }

                
            }
        }
    }
}

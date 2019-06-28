using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Icosphere
{
    [RequireComponent(typeof(Camera))]
    public class CameraHider : MonoBehaviour
    {
        public static HexSphere sphere;
        private Camera viewCamera;
        private int hexRadius = 10;
        private int sphereLayer;
        private static List<HexIdentifier> objects = new List<HexIdentifier>();

        private HashSet<MeshRenderer> previousSet = new HashSet<MeshRenderer>();

        // Start is called before the first frame update
        void Start()
        {
            viewCamera = GetComponent<Camera>();
            sphereLayer = LayerMask.NameToLayer("Sphere");
        }

        public static void AddObject(HexIdentifier obj)
        {
            objects.Add(obj);
            //obj.GetComponent<MeshRenderer>().enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (sphere == null)
                return;

            foreach (MeshRenderer ren in previousSet)
            {
                ren.enabled = false;
            }

            //Debug.DrawRay(viewCamera.transform.position, viewCamera.transform.forward.normalized * 20, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(viewCamera.transform.position, viewCamera.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Sphere")))
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;
                HashSet<MeshRenderer> currentObjects = new HashSet<MeshRenderer>();

                HashSet<SCoord> foundCoordinates = new HashSet<SCoord>();
                LinkedList<SCoord> coordinates = new LinkedList<SCoord>();
                LinkedList<int> distances = new LinkedList<int>();
                coordinates.AddLast(hit.collider.gameObject.GetComponent<HexIdentifier>().location);
                distances.AddLast(0);

                while (coordinates.Count > 0)
                {
                    SCoord coord = coordinates.First.Value;
                    int distance = distances.First.Value;
                    coordinates.RemoveFirst();
                    distances.RemoveFirst();

                    if (foundCoordinates.Contains(coord))
                    {
                        continue;
                    }
                    foundCoordinates.Add(coord);
                    if (distance > hexRadius)
                    {
                        continue;
                    }

                    currentObjects.Add(sphere.GetTile(coord).GetComponent<MeshRenderer>());

                    foreach (SCoord adj in sphere.GetHexMap().GetNeighbors(coord))
                    {
                        coordinates.AddLast(adj);
                        distances.AddLast(distance + 1);
                    }
                }

                foreach (MeshRenderer ren in currentObjects)
                {
                    ren.enabled = true;
                }
                previousSet = currentObjects;
            }
            else
            {
                previousSet = new HashSet<MeshRenderer>();
            }
        }
    }
}

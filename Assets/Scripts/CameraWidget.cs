using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWidget : MonoBehaviour
{
    public GameSphere sphere;
    public Camera camera;
    private Icosphere hexMap;

    private float moveSpeed = 3.0f, rollSpeed = 20.0f, scrollSpeed = 20.0f;

    private SCoord position = new SCoord(0, 0);
    private float height = 5.0f, minHeight = 3.0f, maxHeight = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = sphere.transform.position;
        transform.rotation = sphere.transform.rotation;
        transform.SetParent(sphere.transform);
    }

    // Update is called once per frame
    void Update()
    {
        float rotateZ = -Input.GetAxis("Horizontal");
        float rotateX = Input.GetAxis("Vertical");

        float degreeDistance = Mathf.PI * 2 * sphere.GetSphere().GetHexMap().Radius / 360;

        Vector2 moveVector = new Vector2(rotateX, rotateZ).normalized;
        Vector2 rotateVector = moveVector * moveSpeed / degreeDistance * Time.deltaTime;
        rotateX = rotateVector.x;
        rotateZ = rotateVector.y;

        float rotateY = Time.deltaTime * Input.GetAxis("Roll") * rollSpeed;
        float zoomChange = -Time.deltaTime * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        height = Mathf.Max(minHeight, Mathf.Min(height + zoomChange, maxHeight));

        transform.Rotate(rotateX, rotateY, rotateZ);
        camera.transform.localPosition = new Vector3(0, sphere.GetSphere().GetHexMap().Radius + height, 0);
    }
}

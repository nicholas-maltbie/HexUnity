using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Camera widget to control movement around the sphere.
/// </summary>
public class CameraWidget : MonoBehaviour
{
    /// <summary>
    /// Layer of the sphere for layer masks
    /// </summary>
    int sphereLayer;

    /// <summary>
    /// Sphere that has all the hexes.
    /// </summary>
    public GameSphere sphere;
    /// <summary>
    /// Camera object
    /// </summary>
    public Camera viewCamera;
    /// <summary>
    /// Camera holder
    /// </summary>
    public Transform cameraHolder;

    /// <summary>
    /// Movement controls for the 
    /// </summary>
    private float moveSpeed = 3.0f, rollSpeed = 30.0f, scrollSpeed = 40.0f, mouseSpeed = 20f;

    /// <summary>
    /// Zoom controls for the camera
    /// </summary>
    private float height = 5.0f, minHeight = 3.0f, maxHeight = 8.0f;

    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        // Setup variables and intial position
        sphereLayer = LayerMask.NameToLayer("Sphere");
        transform.position = sphere.transform.position;
        transform.rotation = sphere.transform.rotation;
        transform.SetParent(sphere.transform);

        mousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseNow = Input.mousePosition;

        // How much to move the camera across the player's horizontal axis
        float rotateZ = -Input.GetAxis("Horizontal");
        // How much to move the camera across the player's vertical axis
        float rotateX = Input.GetAxis("Vertical");

        // Distance that each degree covers
        float degreeDistance = Mathf.PI * 2 * sphere.GetSphere().GetHexMap().Radius / 360;

        // Get movement of the mouse
        Vector2 mouseDelta = mouseNow - mousePos;
        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        // Define move vector and normalize it
        Vector2 moveVector = new Vector2(rotateX, rotateZ).normalized;
        // Start translatin that move vector to rotation
        Vector2 rotateVector = moveVector * moveSpeed / degreeDistance * Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            float distanceToScene = height;
            float angularSize = (1 / distanceToScene) * Mathf.Rad2Deg;
            float pixelSize = ((angularSize * Screen.height) / Camera.main.fieldOfView);

            Debug.Log(mouseX + " " + mouseY + " " + pixelSize);

            // Get the movement of the mouse
            Vector2 mouseRotateVector = new Vector2(-mouseY, mouseX) / pixelSize;

            rotateVector = mouseRotateVector;
        }

        // Define the rotation delta in x and y
        rotateX = rotateVector.x;
        rotateZ = rotateVector.y;

        // rotation of the camera
        float rotateY = Time.deltaTime * Input.GetAxis("Roll") * rollSpeed;

        // Delta in the zoom of the camera
        float zoomChange = -Time.deltaTime * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        // Bound the height
        height = Mathf.Max(minHeight, Mathf.Min(height + zoomChange, maxHeight));
        
        // Apply the transformation
        transform.Rotate(rotateX, rotateY, rotateZ);
        cameraHolder.transform.localPosition = new Vector3(0, sphere.GetSphere().GetHexMap().Radius + height, 0);

        mousePos = mouseNow;
    }
}

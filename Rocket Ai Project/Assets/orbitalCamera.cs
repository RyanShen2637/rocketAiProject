using UnityEngine;
using System.Collections.Generic;

public class orbitalCamera : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float zoomSpeed = 12f;
    public float minZoom = 10f;
    public float maxZoom = 300f;
    public float currentZoom = 100f;
    public float xSpeed = 110f;
    public float ySpeed = 110f;

    float x;
    float y;
    Vector3 lastPosition;


    // Initial Camera Values
    Vector3 initialOffset;
    float initialZoom;
    float initialZoomSpeed;
    float initialMinZoom;
    float initialMaxZoom;
    float initialX;
    float initialY;
    float initialXSpeed;
    float initialYSpeed;

    void Awake()
    {
        initialOffset = new Vector3(0f, 35f, 0f);
        initialXSpeed = 110f;
        initialYSpeed = 110f;
        initialZoomSpeed = 12f;
        initialMinZoom = 10f;
        initialMaxZoom = 300f;
        initialZoom = 100f;
        Vector3 angles = transform.eulerAngles;
        initialX = angles.y;
        initialY = angles.x;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            return;
        }

        if (Input.GetMouseButton(0)) {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            resetCamera();
        }

        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0, 0, -currentZoom) + target.position + offset; 

        transform.rotation = rotation;
        transform.position = position;
    }

    void resetCamera()
    {
        x = initialX;
        y = initialY;
        currentZoom = initialZoom;
        xSpeed = initialXSpeed;
        ySpeed = initialYSpeed;
        zoomSpeed = initialZoomSpeed;
        minZoom = initialMinZoom;
        maxZoom = initialMaxZoom;
        offset = initialOffset;

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0, 0, -currentZoom) + target.position + offset;

        transform.rotation = rotation;
        transform.position = position;
    }
}

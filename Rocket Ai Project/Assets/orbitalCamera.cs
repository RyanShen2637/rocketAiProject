using UnityEngine;
using System.Collections.Generic;

public class orbitalCamera : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float currentZoom = 10f;
    public float xSpeed = 100f;
    public float ySpeed = 100f;

    float x;
    float y;
    Vector3 lastPosition;


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

        if (Input.GetMouseButton(0)){
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }

        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0, 0, -currentZoom) + target.position + offset; 

        transform.rotation = rotation;
        transform.position = position;
    }
}

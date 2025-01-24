using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    
    public Transform playerBody;


    float xRotation = 0f;

    public float yRotation, zRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yRotation = transform.localRotation.eulerAngles.y;
        zRotation = transform.localRotation.eulerAngles.z;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);


        playerBody.Rotate(Vector3.up * mouseX);
    }
}

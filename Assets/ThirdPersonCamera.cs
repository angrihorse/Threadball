using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{

    float yaw;
    float pitch;

    public float mouseSensitivity = 10;
    public Transform target;
    public float distanceFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(0, 45);

    public float rotationSmoothTime = 1;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public bool lockCursor;


    // Start is called before the first frame update
    void Start()
    {
        if (lockCursor) {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        transform.position = target.position - transform.forward * distanceFromTarget;
    }
}

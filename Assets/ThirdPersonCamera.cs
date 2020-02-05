using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    float yaw;
    float pitch;

    public Transform target;
    public float mouseSensitivity = 10f;
    public Vector2 minMaxDistanceFromTarget = new Vector2(0.5f, 2f);
    public Vector2 pitchMinMax = new Vector2(0, 45);
    public bool lockCursor;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentRotation = transform.eulerAngles;
        if (lockCursor) {
          Cursor.lockState = CursorLockMode.Locked;
          Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        RaycastHit hit;
        float cameraDistance;
        if (Physics.Raycast(transform.position, -transform.forward, out hit)) {
            cameraDistance = Mathf.Clamp(hit.distance * 0.9f, minMaxDistanceFromTarget.x, minMaxDistanceFromTarget.y);
        } else {
            cameraDistance = minMaxDistanceFromTarget.y;
        }

        transform.eulerAngles = new Vector3(pitch, yaw);
        transform.position = target.position - transform.forward * cameraDistance;
    }
}

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

    public float rotationSmoothTime = 0.05f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public float cameraDistanceSmoothTime = 0.1f;
    float cameraDistanceSmoothVelocity;
    float cameraDistance;

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
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit)) {
            float desiredCameraDistance = Mathf.Clamp(hit.distance * 0.9f, minMaxDistanceFromTarget.x, minMaxDistanceFromTarget.y);
            float playerCameraDist = Vector3.Distance(transform.position, target.position);
            cameraDistance = Mathf.SmoothDamp(playerCameraDist, desiredCameraDistance, ref cameraDistanceSmoothVelocity, cameraDistanceSmoothTime);
        }

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = target.position - transform.forward * cameraDistance;
    }
}

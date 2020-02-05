﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Rigidbody myRigidbody;
    public float moveSpeed = 5;
    Transform mainCamera;
    public float unwindingSpeed = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
        myRigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input != Vector2.zero) {
            Vector2 direction = new Vector2(mainCamera.transform.forward.x, mainCamera.transform.forward.z) * input.y;
            direction += new Vector2(mainCamera.forward.z, -mainCamera.forward.x) * input.x;
            myRigidbody.MovePosition(transform.position + new Vector3(direction.x, 0, direction.y).normalized * moveSpeed * Time.fixedDeltaTime);
            if (transform.localScale.x > 0.1) {
                transform.localScale -= Vector3.one * unwindingSpeed;
            } else {
               Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.name == "Destination(Clone)") {
            Debug.Log("Destination reached.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Player playerScript;

    public Rigidbody rb;
    Transform mainCamera;
    public float moveSpeed = 5;
    public float unwindingSpeed = 0.001f;
    public static int livesCount = 3;
    public Material[] playerMaterials = new Material[3];

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    MazeGenerator mazeGeneratorScript;
    GameObject maze;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
        rb = this.GetComponent<Rigidbody>();
        GameObject maze = GameObject.Find("Maze");
        mazeGeneratorScript = maze.GetComponent<MazeGenerator>();
        GetComponent<Renderer>().material = playerMaterials[livesCount-1];
        foreach (Transform child in transform) {
            if (child.gameObject.name == "Trail") {
                child.GetComponent<TrailRenderer>().material = playerMaterials[livesCount-1];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero) {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
            targetRotation += mainCamera.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
            transform.localScale -= Vector3.one * unwindingSpeed * Time.deltaTime;
        }

        if (transform.localScale.x <= 0.1) {
            foreach (Transform child in transform) {
                if (child.gameObject.name == "Main Camera") {
                    GameObject.Destroy(child.gameObject);
                }
            }
            playerScript.enabled = false;
			transform.localScale = Vector3.zero;
            livesCount--;

            if (livesCount > 0) {
                mazeGeneratorScript.PutPlayerOnStartingPosition();
                GetComponent<Renderer>().material = playerMaterials[livesCount-1];
                foreach (Transform child in transform) {
                    if (child.gameObject.name == "Trail") {
                        child.GetComponent<TrailRenderer>().material = playerMaterials[livesCount];
                    }
                }
            }  else {
                Application.Quit();
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.name == "Destination(Clone)") {
            StartCoroutine(ExecuteAfterTime(1));
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Application.Quit();
    }
}

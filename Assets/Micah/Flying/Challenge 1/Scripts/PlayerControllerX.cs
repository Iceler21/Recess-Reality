using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float verticalInput;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // get the user's vertical input
        verticalInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
        rb.MoveRotation(rb.rotation + verticalInput * rotationSpeed * Time.fixedDeltaTime);
    }
}
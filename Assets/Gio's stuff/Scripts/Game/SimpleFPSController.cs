using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpHeight = 1.5f;
    public float gravity = -19.62f;

    [Header("Camera Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lock and hide the cursor in the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the camera up and down (clamped so you can't flip upside down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the entire player body left and right
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // Check if grounded BEFORE applying new movement
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            // A small constant downward force keeps the controller snapped to the floor
            velocity.y = -2f;
        }

        // Get WASD / Arrow key input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate horizontal movement
        Vector3 move = (transform.right * x + transform.forward * z).normalized * moveSpeed;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine horizontal movement and vertical velocity into ONE Move call
        Vector3 finalMove = new Vector3(move.x, velocity.y, move.z);
        controller.Move(finalMove * Time.deltaTime);
    }
}

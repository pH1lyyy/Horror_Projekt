using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float sprintSpeed = 4f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 1f;
    public float x;
    public float z;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching = false;

    void Start()
    {
        controller = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        float currentSpeed = speed;

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed = sprintSpeed;
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? 1f : 2f;
        }

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 move = player.right * x + player.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (controller.isGrounded)
        {
            velocity.y = -2f;

            if (Input.GetKeyDown(KeyCode.Space) && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            }
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
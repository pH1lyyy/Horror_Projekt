using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float sprintSpeed = 4f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 1f;
    public float bunnyHopBoost = 1.2f; // Bonus prêdkoœci przy bunnyhopie
    public float maxBunnyHopSpeed = 15f; // Limit prêdkoœci bunnyhopa

    private float x;
    private float z;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching = false;
    private bool canBunnyHop = false;

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

        // Bunnyhop mechanic
        if (controller.isGrounded)
        {
            velocity.y = -2f;
            canBunnyHop = true;

            if (Input.GetKeyDown(KeyCode.Space) && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);

                if (canBunnyHop)
                {
                    currentSpeed = Mathf.Min(currentSpeed * bunnyHopBoost, maxBunnyHopSpeed);
                    canBunnyHop = false;
                }
            }
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

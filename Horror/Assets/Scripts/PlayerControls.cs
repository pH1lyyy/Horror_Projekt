using System.Collections;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float sprintSpeed = 4f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 1f;
    public float bunnyHopBoost = 1.2f;
    public float maxBunnyHopSpeed = 15f;

    private float x;
    private float z;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching = false;
    private bool canBunnyHop = false;


    public float currentHealth;
    float maxHealth = 100f;
    private bool isDead = false;

    public Transform startPostion;
    void Start()
    {
        controller = player.GetComponent<CharacterController>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!controller.enabled) return;

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

        if (controller.isGrounded)
        {
            velocity.y = -2f;
            canBunnyHop = true;

            if ((Input.GetKeyDown(KeyCode.Space) || Input.mouseScrollDelta.y < 0) && !isCrouching)
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

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;
      
        Debug.Log("Player has died.");
        StartCoroutine(Respawn());
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f); 
        controller.enabled = false; 
        transform.position = startPostion.position;
        controller.enabled = true;

        yield return new WaitForSeconds(3f);
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log("Player has respawned.");
    }
}

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

    public GameObject loadingScreen;
    public Transform startPostion;
    public GameObject playerHitPanel;
    public AudioSource hitAudioSource;
    public AudioClip hitSound;
    public Transform monster;                
    public Transform monsterStartPosition;

    public AudioClip[] footstepClips;
    public AudioSource footstepAudioSource;
    private float currentFootstepInterval;
    private float baseFootstepInterval = 0.8f;
    private float sprintFootstepMultiplier = 0.55f;  // mniejszy = czêœciej
    private float crouchFootstepMultiplier = 1.5f;  // wiêkszy = rzadziej
    private float nextFootstepTime = 0f;


    void Start()
    {
        controller = player.GetComponent<CharacterController>();
        currentHealth = maxHealth;
        currentFootstepInterval = baseFootstepInterval;

    }

    void Update()
    {
        if (!controller.enabled) return;

        float currentSpeed = speed;
        currentFootstepInterval = baseFootstepInterval;

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed = sprintSpeed;
            currentFootstepInterval = baseFootstepInterval * sprintFootstepMultiplier;
        }

        // Kucanie
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? 1f : 2f;
        }

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
            currentFootstepInterval = baseFootstepInterval * crouchFootstepMultiplier;
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
        PlayFootstepSounds();

    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (hitAudioSource != null && hitSound != null)
        {
            hitAudioSource.PlayOneShot(hitSound);
        }
        if (playerHitPanel != null)
        {
            StartCoroutine(ShowHitPanel());
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    IEnumerator ShowHitPanel()
    {
        if (!isDead && playerHitPanel != null)
        {
            playerHitPanel.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        if (!isDead && playerHitPanel != null)
        {
            playerHitPanel.SetActive(false);
        }
    }


    void Die()
    {
        isDead = true;

        if (playerHitPanel != null)
        {
            playerHitPanel.SetActive(false); 
        }

        Debug.Log("Gracz umar³.");
        GameManager.instance.DecreaseDay();
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);
        loadingScreen.SetActive(true);
        controller.enabled = false;

        transform.position = startPostion.position;
        velocity = Vector3.zero;
        x = 0f;
        z = 0f;

        if (monster != null && monsterStartPosition != null)
        {
            MonsterAI monsterAI = monster.GetComponent<MonsterAI>();
            if (monsterAI != null)
            {
                monsterAI.ResetToStartPosition();
            }
        }

        controller.enabled = true;

        yield return new WaitForSeconds(3f);
        loadingScreen.SetActive(false);
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log("Gracz respawn.");
    }
    void PlayFootstepSounds()
    {
        if (controller.isGrounded && (x != 0 || z != 0) && Time.time >= nextFootstepTime)
        {
            if (footstepClips.Length > 0 && footstepAudioSource != null)
            {
                AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
                footstepAudioSource.PlayOneShot(clip);
                nextFootstepTime = Time.time + currentFootstepInterval;



                float distanceToMonster = Vector3.Distance(transform.position, monster.position);
                if (distanceToMonster < 6.5f)
                {
                    MonsterAI monsterAI = monster.GetComponent<MonsterAI>();
                    if (monsterAI != null)
                    {
                        monsterAI.OnSoundHeard(transform.position);
                    }
                }
            }
        }
    }


}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    [Header("Sound detection")]

    UnityEngine.AI.NavMeshAgent navMeshAgent;

    public float moveSpeed = 3.5f;
    public Transform startPosition;
    private Vector3 soundLocation;
    private bool isReturning = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private bool isDead = false;
    bool SoundHeard = false;

    [Header("Monester states")]
    public Transform player;
    public float detectionRadius = 7f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    float lastAttackTime = 0f;

    private Animator animator;

    [Header("Footstep")]

    public AudioClip[] footstepSounds;
    AudioSource audioSource;
    float footstepInterval = .5f;
    float NextFootstepTime = 0f;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        if (isReturning)
        {
            ReturnToStart();
        }
        else if (isAttacking)
        {
            AttackPlayer();
        }
        else if (isChasing)
        {
            ChasePlayer();
        }
        else if (!isWaiting)
        {
            LookingForPlayer();
        }
        UpdateAnimations();
        PlayFootstepSounds(); 
    }

    public void OnSoundHeard(Vector3 location)
    {
        if (isDead) return;

        soundLocation = location;
        SoundHeard = true;
        isReturning = false;
        isChasing = false;
        isAttacking = false;
        isWaiting = false;
        MoveToSoundLocation();
    }
    void MoveToSoundLocation()
    {
        navMeshAgent.SetDestination(soundLocation);
        if (Vector3.Distance(transform.position, soundLocation) <= navMeshAgent.stoppingDistance)
        {
            StartCoroutine(WaitBeforeReturning());
        }
    }
    private IEnumerator WaitBeforeReturning()
    {
        isWaiting = true;
        yield return new WaitForSeconds(10f);
        isWaiting = false;
        isReturning = true;
        SoundHeard = false;
    }

    void ReturnToStart()
    {
        navMeshAgent.SetDestination(startPosition.position);
        if (Vector3.Distance(transform.position, startPosition.position) <= navMeshAgent.stoppingDistance)
        {
            isReturning = false;
        }
    }
    void LookingForPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                isChasing = true;
                isReturning = false;
                break;
            }
        }
    }
    void ChasePlayer()
    {
        navMeshAgent.SetDestination(player.position);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            navMeshAgent.isStopped = true;
            isChasing = false;
            isAttacking = true;
        }
        else if (distanceToPlayer > detectionRadius)
        {
            isChasing = false;
            isReturning = true;
        }
    }
    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            PlayerControls playerControls = player.GetComponent<PlayerControls>();
            if (playerControls != null)
            {
                playerControls.TakeDamage(50f);
            }
            lastAttackTime = Time.time;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                navMeshAgent.isStopped = false;
                isChasing = true;
                isAttacking = false;
            }
        }
    }
    void UpdateAnimations()
    {
        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isAttacking", isAttacking);
        if (!isMoving && !isAttacking)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }
    void PlayFootstepSounds()
    {
       
            if (navMeshAgent.velocity.magnitude > 0.1f && Time.time >= NextFootstepTime)
            {
                if (footstepSounds.Length > 0)
                {
                    AudioClip footstepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
                    audioSource.PlayOneShot(footstepSound);
                    NextFootstepTime = Time.time + footstepInterval;
                }
            }
        
    }
}

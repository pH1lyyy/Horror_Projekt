using UnityEngine;
using System.Collections;

public class MonsterAI : MonoBehaviour
{

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

    public Transform player;
    public float detectionRadius = 7f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    float lastAttackTime = 0f;

    private Animator animator;


    public AudioClip[] footstepSounds;
    AudioSource audioSource;
    float footstepInterval = .5f;
    float NextFootstepTime = 0f;
    private bool isStunned = false;
    private float stunTimer = 0f;
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private float patrolDelay = 5f;
    private float patrolTimer = 0f;
    private bool patrolStarted = false;
    private bool patrolForward = true;
    private float respawnPatrolDelay = 5f;


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
        if (!patrolStarted)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolDelay)
            {
                patrolStarted = true;
            }
            return;
        }

        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            navMeshAgent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isIdle", true);

            if (stunTimer <= 0f)
            {
                isStunned = false;
                navMeshAgent.isStopped = false;
            }

            return;
        }

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
            if (!isChasing && !SoundHeard) Patrol();
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
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            isReturning = false;

            float minDistance = Mathf.Infinity;
            int closestIndex = 0;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, patrolPoints[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            currentPatrolIndex = closestIndex;
            patrolForward = true;
            Patrol();
        }
        else
        {
            navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
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
    void Patrol()
    {
        if (!patrolStarted) return;

        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (patrolForward)
            {
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolPoints.Length)
                {
                    currentPatrolIndex = patrolPoints.Length - 2;
                    patrolForward = false;
                }
            }
            else
            {
                currentPatrolIndex--;
                if (currentPatrolIndex < 0)
                {
                    currentPatrolIndex = 1;
                    patrolForward = true;
                }
            }

            navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }



    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetBool("isAttacking", true);

            PlayerControls playerControls = player.GetComponent<PlayerControls>();
            if (playerControls != null)
            {
                playerControls.TakeDamage(49f);
            }
            lastAttackTime = Time.time;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            navMeshAgent.isStopped = false;
            isChasing = true;
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }

    void UpdateAnimations()
    {
        if (isDead || isStunned)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            return;
        }

        if (isAttacking)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", true);
            return;
        }

        float speed = navMeshAgent.velocity.magnitude;
        bool isMoving = speed > 0.1f;
        bool isRunning = speed > 1.0f;

        animator.SetBool("isWalking", isMoving && !isRunning);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", !isMoving);
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

    public void ApplyStun(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            stunTimer = duration;
            navMeshAgent.isStopped = true;
            Debug.Log("stun");
        }
    }

    public void ResetToStartPosition()
    {
        navMeshAgent.Warp(startPosition.position);
        isReturning = false;
        isChasing = false;
        isAttacking = false;
        isWaiting = false;
        isStunned = false;
        SoundHeard = false;
        isDead = false;

        navMeshAgent.isStopped = false;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", true);

        patrolStarted = false;
        patrolTimer = 0f;     
        StartCoroutine(WaitBeforePatrolAfterRespawn());
    }
    private IEnumerator WaitBeforePatrolAfterRespawn()
    {
        yield return new WaitForSeconds(respawnPatrolDelay);
        patrolStarted = true;
    }

}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public float moveSpeed = 3.5f;
    public Transform startPosition;
    private Vector3 soundLocation;
    private bool isReturning = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private bool isDead = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
    }

    void Update()
    {
        if (isReturning)
        {
            ReturnToStart();
        }
    }
  
    public void OnSoundHeard(Vector3 location)
    {
        if(isDead) return;
        soundLocation = location;
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
    }

    void ReturnToStart()
    {
        navMeshAgent.SetDestination(startPosition.position);
        if (Vector3.Distance(transform.position, startPosition.position) <= navMeshAgent.stoppingDistance)
        {
            isReturning = false;
        }
    }
}

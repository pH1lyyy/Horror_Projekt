using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorScript : MonoBehaviour
{
    Animator doorAnimator;
    public Transform player;
    public float detectionDistance = 3f;
    public LayerMask playerLayer;
    private bool isPlayerNear = false;
    public string keyLayerName = "";

    private bool isOpen = false;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckPlayerDistance();
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            OnDoorButtonPress();
        }
    }

    void CheckPlayerDistance()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistance, playerLayer))
        {
            if (hit.transform == player)
            {
                isPlayerNear = true;
                return;
            }
        }
        isPlayerNear = false;
    }

    public void OnDoorButtonPress()
    {
        if (!isOpen)
        {
            // sprawdzamy czy jest klucz
            if (!string.IsNullOrEmpty(keyLayerName))
            {
                bool playerHasKey = false;
                foreach (Transform key in player)
                {
                    if (key.gameObject.layer == LayerMask.NameToLayer(keyLayerName))
                    {
                        playerHasKey = true;
                        break;
                    }
                }
                if (!playerHasKey)
                {
                    Debug.Log("You need a key to open this door.");
                    return;
                }
            }

            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            isOpen = true;
        }
    }

    void CloseDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Close");
            isOpen = false;
        }
    }
}

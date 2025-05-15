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
        Vector3 directiontoPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, directiontoPlayer, out hit, detectionDistance, playerLayer))
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
        OpenDoor();
    }
    void OpenDoor()
    {
        if (doorAnimator != null)
        {
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
                if(!playerHasKey)
                {
                    Debug.Log("You need a key to open this door.");
                    return;
                }

            }
            doorAnimator.SetTrigger("Open");
        }
    }
}

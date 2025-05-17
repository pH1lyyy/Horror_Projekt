using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Playerinteraction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Camera playerCamera;
    public GameObject player;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
            {
                WoodPlank woodPlank = hit.collider.GetComponent<WoodPlank>();
                if (woodPlank != null)
                {
                    if (IsHoldingAxe())
                    {
                        woodPlank.Fall();
                    }
                    else
                    {
                        Debug.Log("You need an axe to break this plank.");
                    }
                }
            }
        }

    }
    private bool IsHoldingAxe()
    {
        foreach (Transform child in player.transform)
        {
            if (child.gameObject.layer ==  LayerMask.NameToLayer("Axe"))
            {
                return true;
            }
        }
        return false;
    }
}

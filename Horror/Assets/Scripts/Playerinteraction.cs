using UnityEngine;

public class PlayerInteraction : MonoBehaviour
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
                    if (IsHoldingAxe(player.transform))
                    {
                        woodPlank.Fall();
                    }
                }
            }
        }
    }

    private bool IsHoldingAxe(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Axe") ||
                child.CompareTag("Axe") ||
                child.name.Contains("Axe"))
            {
                return true;
            }


            if (IsHoldingAxe(child))
            {
                return true;
            }
        }
        return false;
    }
}

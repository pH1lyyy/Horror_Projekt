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
                    else
                    {
                        Debug.Log("You need an axe to break this plank.");
                    }
                }
            }
        }
    }

    private bool IsHoldingAxe(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Sprawdzamy warstwê, tag lub nazwê
            if (child.gameObject.layer == LayerMask.NameToLayer("Axe") ||
                child.CompareTag("Axe") ||
                child.name.Contains("Axe"))
            {
                Debug.Log("Gracz trzyma siekierê (znalezione: " + child.name + ")");
                return true;
            }

            // Rekurencyjnie sprawdŸ dzieci
            if (IsHoldingAxe(child))
            {
                return true;
            }
        }
        return false;
    }
}

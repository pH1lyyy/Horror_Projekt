using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;

    public Transform itemHoldPosition;     // dla key
    public Transform itemHoldPosition2;    // dla axe
    public Transform itemHoldPosition3;    // dla card
    public Transform itemHoldPosition4;    // dla flashlight
    public Transform playerBody;

    private GameObject currentItem;

    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip dropSound;
    public Text messageText;
    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            PickupItem();
        }
        if (Input.GetKeyDown(dropKey))
        {
            DropItem();
        }
    }

    void PickupItem()
    {
        if (currentItem != null) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);

        GameObject closestItem = null;
        float closestDistance = pickupRadius;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Item") || hitCollider.CompareTag("Axe") || hitCollider.CompareTag("Card") || hitCollider.CompareTag("Flashlight"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = hitCollider.gameObject;
                }
            }
        }

        if (closestItem != null)
        {
            Pickup(closestItem);
        }
    }

    void Pickup(GameObject item)
    {
        currentItem = item;
        item.transform.SetParent(playerBody);
        int layer = item.layer;
        string layerName = LayerMask.LayerToName(layer);

        Transform holdPoint = itemHoldPosition;

        if (item.CompareTag("Axe"))
        {
            holdPoint = itemHoldPosition2;
        }
        else if (item.CompareTag("Card"))
        {
            holdPoint = itemHoldPosition3;
        }
        else if (item.CompareTag("Flashlight"))
        {
            holdPoint = itemHoldPosition4;
        }

        item.transform.position = holdPoint.position;
        item.transform.rotation = holdPoint.rotation;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null) rb = item.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        ShowMessage($"Podnios³eœ: {layerName}");
    }
    public GameObject GetCurrentItem()
    {
        return currentItem;
    }


    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 2f);
        }
    }

    void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    void DropItem()
    {
        if (currentItem == null) return;

        currentItem.transform.SetParent(null);

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb == null) rb = currentItem.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        currentItem = null;

        if (audioSource != null && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }

        MonsterAI monsterAI = FindObjectOfType<MonsterAI>();
        if (monsterAI != null)
        {
            monsterAI.OnSoundHeard(transform.position);
        }
    }
}

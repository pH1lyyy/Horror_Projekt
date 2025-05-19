using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRadius = 2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;

    public Transform itemHoldPosition;
    public Transform playerBody;  // Przedmiot bêdzie dzieckiem playerBody

    private GameObject currentItem;

    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip dropSound;

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
            if (hitCollider.CompareTag("Item") || hitCollider.CompareTag("Axe"))
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
        // Przypisz parent na playerBody (nie itemHoldPosition)
        item.transform.SetParent(playerBody);

        // Ustaw pozycjê i rotacjê zgodnie z itemHoldPosition
        item.transform.position = itemHoldPosition.position;
        item.transform.rotation = itemHoldPosition.rotation;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null) rb = item.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        // dŸwiêk podnoszenia
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
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

        // dŸwiêk upuszczania
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

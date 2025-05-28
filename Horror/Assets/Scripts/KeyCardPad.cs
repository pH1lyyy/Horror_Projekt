using UnityEngine;

public class KeyCardPad : MonoBehaviour
{
    public AudioClip unlockSound;
    public GameObject gameCompletePanel;

    private AudioSource audioSource;
    private bool isUnlocked = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isUnlocked) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    TryUnlock();
                }
            }
        }
    }

    void TryUnlock()
    {
        PlayerPickup player = FindObjectOfType<PlayerPickup>();
        if (player != null && playerHeldCard(player))
        {
            Unlock();
        }
        else
        {
            Debug.Log("Potrzebujesz karty, aby odblokowaæ panel.");
        }
    }

    bool playerHeldCard(PlayerPickup player)
    {
        GameObject held = typeof(PlayerPickup)
            .GetField("currentItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(player) as GameObject;

        return held != null && held.CompareTag("Card");
    }

    void Unlock()
    {
        isUnlocked = true;
        Debug.Log("Panel odblokowany!");

        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }

        if (WoodPlank.FallenPlanksCount >= 2 && gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
            Debug.Log("Game Complete!");
        }
    }
}


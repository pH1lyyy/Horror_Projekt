using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class KeyCardPad : MonoBehaviour
{
    public AudioClip unlockSound;
    public GameObject gameCompletePanel;

    private AudioSource audioSource;
    private Camera mainCam;
    private bool gameCompleteShown = false;

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
            UsePanel();
        }
    }

    bool playerHeldCard(PlayerPickup player)
    {
        GameObject held = typeof(PlayerPickup)
            .GetField("currentItem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(player) as GameObject;

        return held != null && held.CompareTag("Card");
    }

    void UsePanel()
    {

        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }

        if (!gameCompleteShown && WoodPlank.FallenPlanksCount >= 2 && gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
            gameCompleteShown = true;
            Debug.Log("Game Complete!");
            StartCoroutine(LoadMainMenuAfterDelay(1f));
        }
    }

    IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainMenu");
    }
}

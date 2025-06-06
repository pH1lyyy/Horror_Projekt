using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{
    Animator doorAnimator;
    public Transform player;
    public float detectionDistance = 2f;
    public LayerMask playerLayer;
    private bool isPlayerNear = false;
    public string keyLayerName = "";
    private bool isOpen = false;
    private AudioSource audioSource;
    public AudioClip doorSound;

    public Text messageText;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        doorAnimator = GetComponent<Animator>();
        if (messageText != null)
            messageText.text = "";
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
            if (!string.IsNullOrEmpty(keyLayerName))
            {
                bool playerHasKey = false;
                foreach (Transform child in player)
                {
                    if (child.gameObject.layer == LayerMask.NameToLayer(keyLayerName))
                    {
                        playerHasKey = true;
                        break;
                    }
                }
                if (!playerHasKey)
                {
                    ShowMessage($"Potrzebujesz klucza: {keyLayerName}, aby otworzyæ te drzwi.");
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
            PlayDoorSound();
            ShowMessage("");
        }
    }

    void CloseDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Close");
            isOpen = false;
            PlayDoorSound();
        }
    }

    void PlayDoorSound()
    {
        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
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
}

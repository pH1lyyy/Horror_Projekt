using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int maxDays = 2;
    int currentDay;
    public bool allLocksOpen;

    public Text daysText;
    public GameObject gameOverPanel; 
    public AudioClip ambientClip;   
    private AudioSource audioSource;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ambientClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.05f;
        audioSource.Play();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        currentDay = maxDays;
        UpdateDaysText();
    }

    public void DecreaseDay()
    {
        currentDay--;
        UpdateDaysText();
        if (currentDay <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("Game Over!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    void UpdateDaysText()
    {
        if (daysText != null)
        {
            daysText.text = "Days left " + currentDay;
        }
    }
}

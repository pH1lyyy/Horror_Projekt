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

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);

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
        }

        void UpdateDaysText()
        {
            if (daysText != null)
            {
                daysText.text = "Days left " + currentDay;
            }
        }

    }


using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject infoPanel;
    void Start()
{
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void ShowInfo()
    {
        infoPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        infoPanel.SetActive(false);
    }
}

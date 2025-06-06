using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Drag PausePanel here (Resume/Main/Options/Quit)")]
    public GameObject pausePanel;

    [Header("Drag OptionsPanel here (Volume slider, Back button)")]
    public GameObject optionsPanel;

    public static bool IsPaused;

    void Start()
    {
        // Ensure at runtime both panels start hidden
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If OptionsPanel is open, close it first
            if (optionsPanel.activeSelf)
            {
                CloseOptions();
            }
            // If game is paused but OptionsPanel is closed, resume game
            else if (IsPaused)
            {
                ResumeGame();
            }
            // If game is not paused, open PausePanel
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);    // show main pause UI
        optionsPanel.SetActive(false); // ensure options stays hidden
        Time.timeScale = 0f;
        IsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Called when Options button is clicked
    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Called when ESC is pressed while in OptionsPanel (or if you have a Back button)
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

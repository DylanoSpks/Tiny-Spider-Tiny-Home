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
        // Ensure panels start hidden
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    void Update()
    {
        // Prevent pausing until intro has finished
        if (ControlsIntro.IsIntroActive)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If OptionsPanel is open, close it
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
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);
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

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

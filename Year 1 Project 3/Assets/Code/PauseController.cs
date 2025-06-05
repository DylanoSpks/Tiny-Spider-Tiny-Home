using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenuPanel;    // assign in Inspector
    private bool _isPaused;

    void Update()
    {
        // Listen for the pause key (Escape on PC, for example)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        pauseMenuPanel.SetActive(true);    // show UI
        Time.timeScale = 0f;               // freeze all physics, animations, etc.
        AudioListener.pause = true;        // optionally pause all audio
        Cursor.visible = true;             // show and unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        pauseMenuPanel.SetActive(false);   // hide UI
        Time.timeScale = 1f;               // resume normal time
        AudioListener.pause = false;       // resume audio
        Cursor.visible = false;            // hide/lock cursor again if needed
        Cursor.lockState = CursorLockMode.Locked;
    }

    // These methods can be wired up to your UI Buttons via the Inspector:

    public void OnResumeButtonPressed()
    {
        ResumeGame();
    }

    public void OnQuitButtonPressed()
    {
        // For example, go back to main menu scene:
        Time.timeScale = 1f;               // you want time running when switching scenes
        SceneManager.LoadScene("S");
    }
}
using UnityEngine;

public class ControlsIntro : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject controlsPanel;
    public GameObject pressSpacePanel;

    [Header("Timing")]
    public float introDuration = 5f;

    void Start()
    {
        // 1) Pause everything
        Time.timeScale = 0f;

        // 2) Show the controls panel, hide the “press space” prompt
        controlsPanel.SetActive(true);
        pressSpacePanel.SetActive(false);

        // 3) Begin the intro sequence
        StartCoroutine(IntroSequence());
    }

    private System.Collections.IEnumerator IntroSequence()
    {
        yield return new WaitForSecondsRealtime(introDuration);

        // Show “Press Space to Continue”
        pressSpacePanel.SetActive(true);

        // Wait until Space is pressed
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        controlsPanel.SetActive(false);
        pressSpacePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}